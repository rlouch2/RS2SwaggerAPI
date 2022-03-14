using System;
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
			return filter;
		}
	}

	public class CriteriaGroup : ICriteria
	{
		public List<string> filters { get; private set; }

		private LogicalOperator AndOr { get; set; } = LogicalOperator.And;


		public CriteriaGroup(params ICriteria[] criterias)
		{
			this.filters = new List<string>();
			foreach (Criteria crit in criterias)
			{
				this.filters.Add(crit.filter);
			}
		}

		public CriteriaGroup(LogicalOperator AndOr, params ICriteria[] criterias)
		{
			this.AndOr = AndOr;
			this.filters = new List<string>();
			foreach (Criteria crit in criterias)
			{
				this.filters.Add(crit.filter);
			}
		}

		public string ToQueryString()
		{
			string filter = String.Join(" " + AndOr.ToString() + " ", filters);
			return "(" + filter + ")";
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
