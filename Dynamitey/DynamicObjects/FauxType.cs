using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Reflection;
namespace Dynamitey.DynamicObjects
{


    /// <summary>
    /// A Fake Type
    /// </summary>
    public abstract class FauxType
    {
        /// <summary>
        /// Fauxes the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static implicit operator FauxType(Type type)
        {
            return new RealType(type);
        }


        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <param name="binderName">Name of the binder.</param>
        /// <returns></returns>
        public abstract IEnumerable<MemberInfo> GetMember(string binderName);

        /// <summary>
        /// Gets the contained types.
        /// </summary>
        /// <returns></returns>
        public abstract Type[] GetContainedTypes();

        /// <summary>
        /// Determines whether the specified type contains the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the specified type contains type; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool ContainsType(Type type)
        {
            return GetContainedTypes().Contains(type);
        }

    }


    /// <summary>
    /// A Fake Type that represents a real type
    /// </summary>
    public class RealType : FauxType
    {
        /// <summary>
        /// RealType implicitly conversts to an actualy Type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
         public static implicit operator Type(RealType type)
         {
             return type.TargetType;
         }

         /// <summary>
         /// An actual Type implicitly conversts to a real type
         /// </summary>
         /// <param name="type">The type.</param>
         /// <returns></returns>
         public static implicit operator RealType(Type type)
         {
             return new RealType(type);
         }


         /// <summary>
         /// The target type
         /// </summary>
        protected readonly Type TargetType;

        /// <summary>
        /// Initializes a new instance of the <see cref="RealType" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public RealType(Type type)
        {
            TargetType = type;
        }

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <param name="binderName">Name of the binder.</param>
        /// <returns></returns>
        public override IEnumerable<MemberInfo> GetMember(string binderName)
        {
            return TargetType.GetMember(binderName);
        }

        /// <summary>
        /// Gets the contained types.
        /// </summary>
        /// <returns></returns>
        public override Type[] GetContainedTypes()
        {
            return new[] { TargetType };
        }

   
    }


    /// <summary>
    /// A Fake Tupe that is an aggregate of other types
    /// </summary>
    public class AggreType : FauxType
    {

        /// <summary>
        /// Makes the type appendable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static AggreType MakeTypeAppendable(IEquivalentType type)
        {
            if (type.EquivalentType == null)
            {
                type.EquivalentType = new AggreType();
            }
            if (!(type.EquivalentType is AggreType))
            {
                type.EquivalentType = new AggreType(type.EquivalentType);
            }
            return (AggreType) type.EquivalentType;
        }


       
        private readonly List<FauxType> Types = new List<FauxType>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AggreType" /> class.
        /// </summary>
        /// <param name="types">The types.</param>
        public AggreType(params FauxType[] types)
        {
            Types.AddRange(types);
        }

        /// <summary>
        /// Gets the interface types.
        /// </summary>
        /// <returns></returns>
        public Type[] GetInterfaceTypes()
        {
            return Types.SelectMany(it => it.GetContainedTypes()).Where(it => it.IsInterface).ToArray();
        }

        /// <summary>
        /// Adds the type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void AddType(Type type)
        {
            if (!ContainsType(type))
                Types.Add(type);
        }

        /// <summary>
        /// Adds the type.
        /// </summary>
        /// <param name="type">The type.</param>
        public void AddType(FauxType type)
        {
            if (type is RealType)
            {
                foreach (var realType in type.GetContainedTypes())
                {
                    AddType(realType);
                }
            }else if (type is AggreType)
            {
                foreach (var fauxType in ((AggreType)type).Types)
                {
                    AddType(fauxType);
                }
            }
            else
            {
                Types.Add(type);
            }

        }

        /// <summary>
        /// Gets the members.
        /// </summary>
        /// <param name="binderName">Name of the binder.</param>
        /// <returns></returns>
        public override IEnumerable<MemberInfo> GetMember(string binderName)
        {
            var list = new List<MemberInfo>();
            foreach (FauxType t in Types)
            {
                list.AddRange(t.GetMember(binderName));
            }
            return list;
        }

        /// <summary>
        /// Gets the contained types.
        /// </summary>
        /// <returns></returns>
        public override Type[] GetContainedTypes()
        {
            return Types.SelectMany(it => it.GetContainedTypes()).ToArray();
        }
    }
}
