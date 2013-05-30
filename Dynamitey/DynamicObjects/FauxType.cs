using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Reflection;
namespace Dynamitey.DynamicObjects
{

   
    public abstract class FauxType
    {
        public static implicit operator FauxType(Type type)
        {
            return new RealType(type);
        }


        public abstract IEnumerable<MemberInfo> GetMember(string binderName);

        public abstract Type[] GetContainedTypes();

        public virtual bool ContainsType(Type type)
        {
            return GetContainedTypes().Contains(type);
        }

    }

   
    public class RealType : FauxType
    {
         public static implicit operator Type(RealType type)
         {
             return type.TargetType;
         }

         public static implicit operator RealType(Type type)
         {
             return new RealType(type);
         }

       
        protected readonly Type TargetType;

        public RealType(Type type)
        {
            TargetType = type;
        }

        public override IEnumerable<MemberInfo> GetMember(string binderName)
        {
            return TargetType.GetMember(binderName);
        }

        public override Type[] GetContainedTypes()
        {
            return new[] { TargetType };
        }

   
    }
       
   
    public class AggreType : FauxType
    {

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


       
        protected readonly List<FauxType> Types = new List<FauxType>();

        public AggreType(params FauxType[] types)
        {
            Types.AddRange(types);
        }

        public Type[] GetInterfaceTypes()
        {
            return Types.SelectMany(it => it.GetContainedTypes()).Where(it => it.IsInterface).ToArray();
        }

        public void AddType(Type type)
        {
            if (!ContainsType(type))
                Types.Add(type);
        }

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

        public override IEnumerable<MemberInfo> GetMember(string binderName)
        {
            var list = new List<MemberInfo>();
            foreach (FauxType t in Types)
            {
                list.AddRange(t.GetMember(binderName));
            }
            return list;
        }

        public override Type[] GetContainedTypes()
        {
            return Types.SelectMany(it => it.GetContainedTypes()).ToArray();
        }
    }
}
