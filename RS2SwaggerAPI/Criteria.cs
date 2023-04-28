using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RS2SwaggerAPI
{
    public class Criteria : ICriteria
    {
        public string filter { get; set; }
        public Criteria(string field, string value)
        {
            filter = field + " eq " + value;
        }

        public Criteria(string field, string value, ComparisonOperators comparisonOperator)
        {
            filter = field + " " + comparisonOperator.ToString() + " " + value;
        }

        public string ToQueryString()
        {
            //CriteriaGroup group = new CriteriaGroup(this);
            //return group.ToQueryString(); 
            if (filter != null)
                return filter;
            else
                return "";
        }
    }

    public class CriteriaGroup : ICriteria
    {

        public List<string> filters { get; private set; }

        private LogicalOperator AndOr { get; set; } = LogicalOperator.And;

        public CriteriaGroup(params ICriteria[] criterias)
        {
            this.filters = new List<string>();

            foreach (var critList in criterias)
            {
                if (critList.GetType() == typeof(CriteriaGroup))
                {
                    CriteriaGroup critGroup = (CriteriaGroup)critList;
                    if (critGroup.filters.Count > 0)
                        foreach (string strFilter in critGroup.filters)
                        {
                            this.filters.Add(strFilter);
                        }

                    string filter = String.Join(" And ", filters);

                    filters.Clear();
                    filters.Add(filter);
                }
                else
                {
                    Criteria crit = (Criteria)critList;
                    this.filters.Add(crit.filter);
                }
                string finalFilter = String.Join(" And ", filters);
                finalFilter = "(" + finalFilter + ")";

                filters.Clear();
                filters.Add(finalFilter);
            }


        }

        public CriteriaGroup(LogicalOperator AndOr, params ICriteria[] criterias)
        {
            this.AndOr = AndOr;
            this.filters = new List<string>();

            foreach (var critList in criterias)
            {
                if (critList.GetType() == typeof(CriteriaGroup))
                {
                    CriteriaGroup critGroup = (CriteriaGroup)critList;
                    if (critGroup.filters.Count > 0)
                    {
                        foreach (string strFilter in critGroup.filters)
                        {
                            this.filters.Add("(" + strFilter + ")");
                        }

                        string filter = String.Join(" " + critGroup.AndOr + " ", filters);
                        filters.Clear();
                        filters.Add(filter);
                    }
                    else
                        new CriteriaGroup(critGroup.AndOr, critGroup);

                }
                else
                {
                    Criteria crit = (Criteria)critList;
                    this.filters.Add(crit.filter);
                }

                string finalFilter = String.Join(" " + AndOr + " ", filters);
                finalFilter = "(" + finalFilter + ")";

                filters.Clear();
                filters.Add(finalFilter);
            }

        }

        public string ToQueryString()
        {
            if (filters.Count > 0)
            {
                string filter = String.Join(" " + AndOr.ToString() + " ", filters);
                return "(" + filter + ")";
            }
            else
                return "";
        }
    }


    public interface ICriteria
    {

        string ToQueryString();
    }

    public enum ComparisonOperators
    {
        [Description("Equal")]
        Eq,
        [Description("Greater than")]
        Gt,
        [Description("Less than")]
        Lt,
        [Description("Not equals")]
        Ne,
        [Description("Greather than or equal to")]
        Ge,
        [Description("Less than or equal to")]
        Le,
    }

    public enum LogicalOperator
    {
        And,
        Or
    }
}
