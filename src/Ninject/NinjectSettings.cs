//-------------------------------------------------------------------------------------------------
// <copyright file="NinjectSettings.cs" company="Ninject Project Contributors">
//   Copyright (c) 2007-2010, Enkari, Ltd.
//   Copyright (c) 2010-2016, Ninject Project Contributors
//   Authors: Nate Kohari (nate@enkari.com)
//            Remo Gloor (remo.gloor@gmail.com)
//
//   Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
//   you may not use this file except in compliance with one of the Licenses.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//   or
//       http://www.microsoft.com/opensource/licenses.mspx
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Ninject
{
    using System;
    using System.Collections.Generic;
    using Ninject.Activation;
    using Ninject.Infrastructure;

    /// <summary>
    /// Contains configuration options for Ninject.
    /// </summary>
    public class NinjectSettings : INinjectSettings
    {
        private readonly IDictionary<string, object> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectSettings"/> class.
        /// </summary>
        public NinjectSettings()
            : this(new Dictionary<string, object>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectSettings"/> class.
        /// </summary>
        /// <param name="values">Dependency injection for the settings values</param>
        private NinjectSettings(IDictionary<string, object> values)
        {
            this.values = values;
#if NO_LCG
            this.UseReflectionBasedInjection = false;
#endif
        }

        /// <summary>
        /// Gets or sets the attribute that indicates that a member should be injected.
        /// </summary>
        public Type InjectAttribute
        {
            get { return this.Get("InjectAttribute", typeof(InjectAttribute)); }
            set { this.Set("InjectAttribute", value); }
        }

        /// <summary>
        /// Gets or sets the attribute that indicates that a member is obsolete and should not be injected.
        /// </summary>
        public Type ObsoleteAttribute
        {
            get { return this.Get("ObsoleteAttribute", typeof(ObsoleteAttribute)); }
            set { this.Set("ObsoleteAttribute", value); }
        }

        /// <summary>
        /// Gets or sets the interval at which the GC should be polled.
        /// </summary>
        public TimeSpan CachePruningInterval
        {
            get { return this.Get("CachePruningInterval", TimeSpan.FromSeconds(30)); }
            set { this.Set("CachePruningInterval", value); }
        }

        /// <summary>
        /// Gets or sets the default scope callback.
        /// </summary>
        public Func<IContext, object> DefaultScopeCallback
        {
            get { return this.Get("DefaultScopeCallback", StandardScopeCallbacks.Transient); }
            set { this.Set("DefaultScopeCallback", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the kernel should automatically load extensions at startup.
        /// </summary>
        public bool LoadExtensions
        {
            get { return this.Get("LoadExtensions", true); }
            set { this.Set("LoadExtensions", value); }
        }

        /// <summary>
        /// Gets or sets the paths that should be searched for extensions.
        /// </summary>
        public string[] ExtensionSearchPatterns
        {
            get { return this.Get("ExtensionSearchPatterns", new[] { "Ninject.Extensions.*.dll", "Ninject.Web*.dll" }); }
            set { this.Set("ExtensionSearchPatterns", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should use reflection-based injection instead of
        /// the (usually faster) lightweight code generation system.
        /// </summary>
        public bool UseReflectionBasedInjection
        {
            get { return this.Get("UseReflectionBasedInjection", false); }
            set { this.Set("UseReflectionBasedInjection", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should inject non public members.
        /// </summary>
        public bool InjectNonPublic
        {
            get { return this.Get("InjectNonPublic", false); }
            set { this.Set("InjectNonPublic", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Ninject should inject private properties of base classes.
        /// </summary>
        /// <remarks>
        /// Activating this setting has an impact on the performance. It is recommended not
        /// to use this feature and use constructor injection instead.
        /// </remarks>
        public bool InjectParentPrivateProperties
        {
            get { return this.Get("InjectParentPrivateProperties", false); }
            set { this.Set("InjectParentPrivateProperties", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the activation cache is disabled.
        /// If the activation cache is disabled less memory is used. But in some cases
        /// instances are activated or deactivated multiple times. e.g. in the following scenario:
        /// Bind{A}().ToSelf();
        /// Bind{IA}().ToMethod(ctx =&gt; kernel.Get{IA}();
        /// </summary>
        /// <value>
        ///     <c>true</c> if activation cache is disabled; otherwise, <c>false</c>.
        /// </value>
        public bool ActivationCacheDisabled
        {
            get { return this.Get("ActivationCacheDisabled", false); }
            set { this.Set("ActivationCacheDisabled", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Null is a valid value for injection.
        /// By default this is disabled and whenever a provider returns null an exception is thrown.
        /// </summary>
        /// <value>
        ///     <c>true</c> if null is allowed as injected value otherwise false.
        /// </value>
        public bool AllowNullInjection
        {
            get { return this.Get("AllowNullInjection", false); }
            set { this.Set("AllowNullInjection", value); }
        }

        /// <summary>
        /// Gets the value for the specified key.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="key">The setting's key.</param>
        /// <param name="defaultValue">The value to return if no setting is available.</param>
        /// <returns>The value, or the default value if none was found.</returns>
        public T Get<T>(string key, T defaultValue)
        {
            object value;
            return this.values.TryGetValue(key, out value) ? (T)value : defaultValue;
        }

        /// <summary>
        /// Sets the value for the specified key.
        /// </summary>
        /// <param name="key">The setting's key.</param>
        /// <param name="value">The setting's value.</param>
        public void Set(string key, object value)
        {
            this.values[key] = value;
        }

        /// <summary>
        /// Clones the ninject settings into a new instance
        /// </summary>
        /// <returns>A new instance of the ninject settings</returns>
        public INinjectSettings Clone()
        {
            var clonedValues = new Dictionary<string, object>(this.values);
            return new NinjectSettings(clonedValues);
        }
    }
}