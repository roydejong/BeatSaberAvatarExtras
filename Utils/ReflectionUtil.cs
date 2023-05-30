using System;
using System.Reflection;

namespace BeatSaberAvatarExtras.Utils
{
    public static class ReflectionUtil
    {
        /// <summary>
        /// Invokes a method from <typeparamref name="T" /> on an object with a generic type.
        /// </summary>
        /// <typeparam name="U">the type that the method returns</typeparam>
        /// <typeparam name="T">the type to search for the method on</typeparam>
        /// <typeparam name="T">the generic type to invoke the method with</typeparam>
        /// <param name="obj">the object instance</param>
        /// <param name="methodName">the method's name</param>
        /// <param name="args">the method arguments</param>
        /// <returns>the return value</returns>
        /// <exception cref="T:System.MissingMethodException">if <paramref name="methodName" /> does not exist on <typeparamref name="T" /></exception>
        public static U InvokeGenericMethod<U, T, G>(this T obj, string methodName, params object[] args)
        {
            MethodInfo method = typeof (T).GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (method == (MethodInfo) null)
                throw new MissingMethodException("Method " + methodName + " does not exist", nameof (methodName));
            MethodInfo generic = method.MakeGenericMethod(typeof(G));
            return (U) generic?.Invoke((object) obj, args);
        }
    }
}