using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Synchronization.Core;

namespace Synchronization.Models
{
    /// <summary>
    /// Reflection helper methods.
    /// </summary>
    internal static class Rhm
    {
        #region Extension methods

        /// <summary>
        /// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types
        /// (generic types without any type parameters).
        /// </summary>
        /// <param name="searchForType">The base type class for which the check is made.</param>
        /// <param name="type">The type to determine for whether it derives from <paramref name="searchForType"/>.</param>
        /// <param name="baseType">The implementation of the generic class definition of the <paramref name="searchForType"/>.</param>
        public static bool IsSubclassOfRawGeneric(this Type type, Type searchForType, out Type baseType)
        {
            while (type != typeof(object))
            {
                Type cur = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
                if (cur == searchForType)
                {
                    baseType = type;
                    return true;
                }
                type = type.BaseType;
            }
            baseType = null;
            return false;
        }

        #region IsWorker

        /// <summary>
        /// Specifies whether some type implements IWorker of specific tickets.
        /// </summary>
        /// <param name="type">the type to check</param>
        /// <param name="tTicketIn">the input ticket type</param>
        /// <param name="tTicketOut">the output ticket type</param>
        /// <returns>whether the class implements the IWorker interface</returns>
        public static bool IsWorkerOf(this Type type, Type tTicketIn, Type tTicketOut)
        {
            return type.IsWorkerArguments().Any(a => a.Item1 == tTicketIn && a.Item2 == tTicketOut);
        }

        public static List<Tuple<FieldInfo[], FieldInfo[]>> AnalyseIsWorker(this Type type)
        {
            return type.IsWorkerArguments().Select(i => new Tuple<FieldInfo[], FieldInfo[]>
                                  (i.Item1.GetChannels(), i.Item2.GetChannels())).ToList();
        }

        private static IEnumerable<Tuple<Type, Type>> IsWorkerArguments(this Type type)
        {
            return from interfaceType in type.GetInterfaces()
                   where interfaceType.IsGenericType
                   select interfaceType.GetGenericTypeDefinition()
                       into baseInterface
                       where baseInterface == WorkerInterfaceGen
                       select baseInterface.GetGenericArguments()
                           into args
                           select new Tuple<Type, Type>(args[0], args[1]);
        }

        #endregion // End of IsWorker

        private static FieldInfo[] GetChannels(this Type tTicket)
        {
            FieldInfo[] fields = tTicket.GetFields(BindingFlags.Public | BindingFlags.Instance),
                        channels = fields.Where(f => f.GetCustomAttributes
                            (typeof(ChannelAttribute), true).Length != 0).ToArray();

            return channels.Any() ? channels : fields;
        }

        public static string PrettyName(this Type type)
        {
            if (type == typeof(int))
                return "int";
            if (type == typeof(string))
                return "string";

            var result = PrettyTypeName(type);
            if (type.IsGenericType)
                result = result + PrettyNameForGeneric(type.GetGenericArguments());
            return result;
        }

        #endregion // End of Extension methods

        #region Pretty names

        private static string PrettyTypeName(Type type)
        {
            var result = type.Name;
            if (type.IsGenericType)
                result = result.Remove(result.IndexOf('`'));

            if (type.IsNested && !type.IsGenericParameter)
                return type.DeclaringType.PrettyName() + "." + result;

            if (type.Namespace == null)
                return result;

            var conflictingTypes = ReferencedTypes
                .Where(definedType => definedType.Name == type.Name && definedType.Namespace != type.Namespace)
                .ToArray();

            var namespaceParts = type.Namespace.Split('.').Reverse().ToArray();
            var namespacePart = "";
            for (var i = 0; i < namespaceParts.Length && conflictingTypes.Length > 0; i++)
            {
                namespacePart = namespaceParts[i] + "." + namespacePart;
                conflictingTypes = conflictingTypes
                    .Where(conflictingType => (conflictingType.Namespace + ".").EndsWith(namespacePart))
                    .ToArray();
            }

            return namespacePart + result;
        }

        private static IEnumerable<Type> _referencedTypesCache;

        private static IEnumerable<Type> ReferencedTypes
        {
            get
            {
                if (_referencedTypesCache == null)
                    _referencedTypesCache = Assembly.GetEntryAssembly().GetTypes();
                return _referencedTypesCache;
            }
        }

        private static string PrettyNameForGeneric(Type[] types)
        {
            var result = "";
            var delim = "<";
            foreach (var t in types)
            {
                result += delim;
                delim = ",";
                result += t.PrettyName();
            }
            return result + ">";
        }

        #endregion // End of Pretty names

        #region Creation methods

        /// <summary>
        /// Creates a new thread-safe pair of I/O queues.
        /// </summary>
        /// <param name="tInOutQueue">pair of I/O queues types</param>
        /// <param name="tInQueue">input queue type</param>
        /// <param name="tOutQueue">output queue type</param>
        /// <param name="queueIn">input queue instanse</param>
        /// <param name="queueOut">output queue instanse</param>
        /// <returns>an instance of an IOqueues closed type</returns>
        public static object CreateInOutQueuesInstanse(Type tInOutQueue, Type tInQueue, Type tOutQueue,
                                                       object queueIn, object queueOut)
        {
            ConstructorInfo qci = tInOutQueue.GetConstructor(new[] { tInQueue, tOutQueue });
            return qci.Invoke(new[] { queueIn, queueOut });
        }

        /// <summary>
        /// Creates the type for a thread-safe pair of I/O queues.
        /// </summary>
        /// <param name="tTicketIn">input queue ticket type</param>
        /// <param name="tTicketOut">output queue ticket type</param>
        /// <returns>the type for the pair of I/O queues</returns>
        public static Type CreateInOutQueuesType(Type tTicketIn, Type tTicketOut)
        {
            return InOutQueuesGen.MakeGenericType(new[] { tTicketIn, tTicketOut });
        }

        /// <summary>
        /// Creates a new thread-safe queue of type <paramref name="tQueue"/>.
        /// </summary>
        /// <param name="tQueue">the type of the queue</param>
        /// <param name="node">the node to be wakened on ticket entry if necessery</param>
        /// <returns>the queue instanse</returns>
        public static object CreateTsQueueInstanse(Type tQueue, IWakeable node = null)
        {
            ConstructorInfo qci = tQueue.GetConstructor(Type.EmptyTypes);
            return qci.Invoke(node == null ? new object[0] : new object[] { node });
        }

        /// <summary>
        /// Creates the type of the thread-safe queue for the type <paramref name="tTicket"/>.
        /// </summary>
        /// <param name="tTicket">queue object type</param>
        /// <returns>the type of the TSQueue for the type <paramref name="tTicket"/></returns>
        public static Type CreateTsQueueType(Type tTicket)
        {
            return TsQueueGen.MakeGenericType(new[] { tTicket });
        }

        /// <summary>
        /// Creates the type for job tickets of type <paramref name="tTicket"/>.
        /// </summary>
        /// <param name="tTicket">queue object type</param>
        /// <returns>the type of the job tickets</returns>
        public static Type CreateJobTicketType(Type tTicket)
        {
            return JobTicketGen.MakeGenericType(new[] { tTicket });
        }

        #endregion // End of Creation methods

        #region Generic type definitions

        /// <summary>
        /// Thread-safe queue generic type definition.
        /// </summary>
        private static readonly Type TsQueueGen = typeof(TsQueue<>);

        /// <summary>
        /// Pair of I/O queues generic type definition.
        /// </summary>
        private static readonly Type InOutQueuesGen = typeof(InOutQueues<,>);

        /// <summary>
        /// IWorker generic type definition.
        /// </summary>
        private static readonly Type WorkerInterfaceGen = typeof(IWorker<,>);

        /// <summary>
        /// Job ticket generic type definition.
        /// </summary>
        private static readonly Type JobTicketGen = typeof(JobTicket<>);

        #endregion // End of Generic type definitions
    }
}
