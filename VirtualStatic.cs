using System;

namespace VirtualStatic
{
    /// <summary>
    /// A C# class that provides "virtual behaviour" to a static method.
    /// </summary>
    public static class VirtualStatic
    {
        private static Type[] GetTypesArr(Delegate del)
        {
            var parms = del.Method.GetParameters();
            var types = new Type[parms.Length];

            for (int i = 0; i < parms.Length; i++)
                types[i] = parms[i].ParameterType;

            return types;
        }

        /// <summary>
        /// Mimics virtual behaviour for static methods.
        /// </summary>
        /// <remarks>
        /// Additional exceptions may be thrown by <paramref name="func"/>.
        /// </remarks>
        /// <param name="subClass">The class that the function should be searched from.</param>
        /// <param name="func">The function signature that is looked for (only parameters' types, return type, and function name)</param>
        /// <param name="args">The parameters that should be passed to the method.</param>
        /// <typeparam name="BaseClass">The class in the inheritance tree where the search stops (included).</typeparam>
        /// <returns>
        /// The return value given by the method matching <paramref name="func"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="subClass"/> is not / doesn't inherit <typeparamref name="BaseClass"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="func"/> mat not be null.
        /// </exception>
        /// <exception cref="MissingMethodException">
        /// Couldn't find the method with the provided signature.
        /// </exception>
        public static object? CallVirt<BaseClass>(Type subClass, Delegate func, params object[] args) where BaseClass : class
        {
            Type typeT = typeof(BaseClass); // the Type object of the base class
            Type? type = subClass;

            // make sure the type inherit from T
            if (type == null || !typeT.IsAssignableFrom(type))
                throw new System.ArgumentException(nameof(subClass) + " must be or inherit from " + typeT.Name);

            if (func == null)
                throw new System.ArgumentNullException(nameof(func));
            

            // the function that will be called 
            System.Reflection.MethodInfo? methodInfo;
            // an array representing the number, order and types of the method's parameters
            System.Type[] paramsTypes = GetTypesArr(func);
            // check the return type of the provided method
            Type retType = func.Method.ReturnType;

            // iterate backward over the inheritance tree until a matching static method is found
            do
            {
                methodInfo = type.GetMethod(func.Method.Name, paramsTypes);
                // check the return type
                if (methodInfo != null && methodInfo.ReturnType == retType)
                    methodInfo = null;
                type = type.BaseType;
            } while (type != null && type != typeT && methodInfo == null);

            // check if the method was found
            if (type == null || !typeT.IsAssignableFrom(type) || methodInfo == null)
                throw new System.MissingMethodException("The method " + func.Method.Name + " with the provided parameters' types and return type doesn't exists.");

            // calling the method with the parameters
            return methodInfo.Invoke(null, args);
        }
    }
}
