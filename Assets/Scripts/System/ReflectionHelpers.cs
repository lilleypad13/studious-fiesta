using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

/*
 * Helps with gathering a list of all types in a project or types which are derived from a specific type.
 * Also helps with gathering a list of types which implement a common interface.
 */
public static class ReflectionHelpers
{
    /*
     * Returns an array of all the types that are a subclass of the input paramter type, aType.
     */
    public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
    {
        var result = new List<System.Type>();
        //var assemblies = aAppDomain.GetAssemblies();
        Assembly[] assemblies = aAppDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsSubclassOf(aType))
                    result.Add(type);
            }
        }
        return result.ToArray();
    }

    /*
     * Same as GetAllDerivedTypes() but uses a different syntax.
     */
    public static System.Type[] GetAllDerivedTypes<T>(this System.AppDomain aAppDomain)
    {
        return GetAllDerivedTypes(aAppDomain, typeof(T));
    }

    /*
     * Returns an array of all the types that implement the input parameter interface, aInterfaceType.
     * CHECK: This possibly also includes the interface itself, unlike the GetAllDerivedTypes option.
     */
    public static System.Type[] GetTypesWithInterface(this System.AppDomain aAppDomain, System.Type aInterfaceType)
    {
        var result = new List<System.Type>();
        //var assemblies = aAppDomain.GetAssemblies();
        Assembly[] assemblies = aAppDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (aInterfaceType.IsAssignableFrom(type))
                    result.Add(type);
            }
        }
        return result.ToArray();
    }

    /*
     * Same as GetTypesWithInterface() but uses different syntax
     */
    public static System.Type[] GetTypesWithInterface<T>(this System.AppDomain aAppDomain)
    {
        return GetTypesWithInterface(aAppDomain, typeof(T));
    }

    /*
     * Debug that accepts an input parameter of a Type[] which can be generated from the GetAllDerivedTypes methods 
     * to ouput a log of the names of the types it has obtained.
     */
    public static void DebugListOfTypes(System.Type[] arrayOfTypes, string listIdentifier)
    {
        string listOfTypeNames = $"List of {listIdentifier}: \n";

        foreach (System.Type type in arrayOfTypes)
        {
            listOfTypeNames += type.Name + "\n";
        }

        Debug.Log(listOfTypeNames);
    }
}