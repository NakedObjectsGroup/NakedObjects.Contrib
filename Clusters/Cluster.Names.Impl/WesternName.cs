using NakedObjects;

namespace Cluster.Names.Impl
{
    public class WesternName : AbstractName
    {

        public override string ToString()
        {
            TitleBuilder t = new TitleBuilder();
            t.Append(NormalName);
            return t.ToString();
        }


        #region Properties
        [Optionally, MemberOrder(10)]
        public virtual WesternNamePrefixes? Prefix { get; set; }

        //TODO: Add Regex for alpha & punctuation only
        [MemberOrder(20)]
        public virtual string FirstName { get; set; }

        [Optionally, MemberOrder(30)]
        public virtual string InformalFirstName { get; set; }

        [Optionally, MemberOrder(40)]
        public virtual string MiddleInitial { get; set; }

        [MemberOrder(50)]
        public virtual string LastName { get; set; }

        [Optionally, MemberOrder(60)]
        public virtual string Suffix { get; set; }

        #endregion

        #region IName properties
        [Disabled, MemberOrder(100)]
        public override string NormalName
        {
            get
            {
                TitleBuilder t = new TitleBuilder();
                t.Append(FirstName).Append(LastName);
                return t.ToString();
            }
        }

        [Disabled, MemberOrder(110)]
        public override string FormalName
        {
            get
            {
                TitleBuilder t = new TitleBuilder();
                t.Append(Prefix).Append(FirstName).Append(MiddleInitial).Append(LastName).Append(Suffix);
                return t.ToString();
            }
        }

        [Disabled, MemberOrder(120)]
        public override string SortableName
        {
            get
            {
                TitleBuilder t = new TitleBuilder();
                t.Append(LastName).Append(",", FirstName).Append(MiddleInitial);
                return t.ToString();
            }
        }

        [Disabled, MemberOrder(130)]
        public override string FormalSalutation
        {
            get
            {
                TitleBuilder t = new TitleBuilder();
                t.Append(Prefix);
                if (Prefix == null) t.Append(FirstName);
                t.Append(LastName);
                return t.ToString();
            }
        }

        [Disabled, MemberOrder(140)]
        public override string InformalSalutation
        {
            get
            {
                return InformalFirstName ?? FirstName;
            }
        }
        #endregion
    }

    public enum WesternNamePrefixes
    {
        Mr, Mrs, Ms, Miss, Dr, Prof, Hon
    }
}
