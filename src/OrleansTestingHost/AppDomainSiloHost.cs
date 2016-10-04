﻿using System;
using System.Collections.Generic;
using Orleans.CodeGeneration;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;

namespace Orleans.TestingHost
{
    /// <summary>Allows programmatically hosting an Orleans silo in the curent app domain, exposing some marshable members via remoting.</summary>
    public class AppDomainSiloHost : MarshalByRefObject
    {
        private readonly Silo silo;

        /// <summary>Creates and initializes a silo in the current app domain.</summary>
        /// <param name="name">Name of this silo.</param>
        /// <param name="siloType">Type of this silo.</param>
        /// <param name="config">Silo config data to be used for this silo.</param>
        public AppDomainSiloHost(string name, Silo.SiloType siloType, ClusterConfiguration config)
        {
            this.silo = new Silo(name, siloType, config);
        }

        /// <summary> SiloAddress for this silo. </summary>
        public SiloAddress SiloAddress => silo.SiloAddress;

        /// <summary>Gets the Silo test hook (NOTE: this will be removed really soon, and was migrated here temporarily)</summary>
        public Silo.TestHooks TestHook => silo.TestHook;

        /// <summary>Methods for optimizing the code generator.</summary>
        public class CodeGeneratorOptimizer : MarshalByRefObject
        {
            /// <summary>Adds a cached assembly to the code generator.</summary>
            /// <param name="targetAssemblyName">The assembly which the cached assembly was generated for.</param>
            /// <param name="cachedAssembly">The generated assembly.</param>
            public void AddCachedAssembly(string targetAssemblyName, GeneratedAssembly cachedAssembly)
            {
                CodeGeneratorManager.AddGeneratedAssembly(targetAssemblyName, cachedAssembly);
            }
        }

        /// <summary>Represents a collection of generated assemblies accross an application domain.</summary>
        public class GeneratedAssemblies : MarshalByRefObject
        {
            /// <summary>Initializes a new instance of the <see cref="GeneratedAssemblies"/> class.</summary>
            public GeneratedAssemblies()
            {
                Assemblies = new Dictionary<string, GeneratedAssembly>();
            }

            /// <summary>Gets the assemblies which were produced by code generation.</summary>
            public Dictionary<string, GeneratedAssembly> Assemblies { get; }

            /// <summary>Adds a new assembly to this collection.</summary>
            /// <param name="key">The full name of the assembly which code was generated for.</param>
            /// <param name="value">The raw generated assembly.</param>
            public void Add(string key, GeneratedAssembly value)
            {
                if (!string.IsNullOrWhiteSpace(key))
                {
                    Assemblies[key] = value;
                }
            }
        }

        /// <summary>
        /// Populates the provided <paramref name="collection"/> with the assemblies generated by this silo.
        /// </summary>
        /// <param name="collection">The collection to populate.</param>
        public void UpdateGeneratedAssemblies(GeneratedAssemblies collection)
        {
            var generatedAssemblies = CodeGeneratorManager.GetGeneratedAssemblies();
            foreach (var asm in generatedAssemblies)
            {
                collection.Add(asm.Key, asm.Value);
            }
        }

        /// <summary>Starts the silo</summary>
        public void Start()
        {
            silo.Start();
        }

        /// <summary>Gracefully shuts down the silo</summary>
        public void Shutdown()
        {
            silo.Shutdown();
        }
    }
}