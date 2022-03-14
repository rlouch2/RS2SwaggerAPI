using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Xml.Linq;

using Newtonsoft.Json.Linq;
using System.Dynamic;

namespace RS2SwaggerAPI
{
	//public class JsonApiObjectList
	//{
	//	public List<JsonApiObject> JsonApiObject { get; set; }
	//}

	public class JsonApiObject : DynamicObject
	{
		#region Declarations

		//[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private JToken m_token;

		#endregion Declarations

		#region Properties

		/// <summary>
		/// Gets the name of the source table for the record that this object represents
		/// </summary>
		public string Table { get; set; }

		//public int ColumnCount
		//{
		//	get
		//	{
		//		return ((JObject)m_token).Properties().Count();
		//	}
		//}

		//public Dictionary<string, string> Columns
		//{
		//	get
		//	{
		//		Dictionary<string, string> values = new Dictionary<string, string>();
		//		foreach (string col in GetDynamicMemberNames())
		//		{
		//			values.Add(col, m_token[col].ToString());
		//		}

		//		return values;
		//	}
		//}

		#endregion Properties

		#region Constructor

		internal JsonApiObject(JToken token, string tableName)
		{
			if (token == null) throw new ArgumentNullException("token");
			Table = tableName;
			m_token = token;
		}

		public JsonApiObject() { }

		#endregion Constructor

		#region DynamicObject Overrides

		/// <summary>
		/// Determines whether the specified field exists in this object.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public bool HasField(string name)
		{
			return m_token[name].Any();
			//return m_element.Elements(name).Any();
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			//int count = m_token[binder.Name].Parent.Count();

			result = null;
			//if (m_token[binder.Name].Any())
			//{
			switch (m_token[binder.Name].Type)
			{
				case JTokenType.Object:
					if (m_token[binder.Name].HasValues)
					{
						result = m_token[binder.Name];
					}
					else
						result = m_token[binder.Name].ToString();

					break;
				case JTokenType.Array:
					JArray ary = JArray.Parse(m_token[binder.Name].ToString());

					result = ary;
					break;
				case JTokenType.String:
					result = m_token[binder.Name].ToString();
					break;
				default:
					result = m_token[binder.Name];
					break;
			}
			//}
			//else
			//	result = null;

			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			m_token[binder.Name] = JToken.FromObject(value);


			//switch (m_token[binder.Name].Type)
			//{
			//	case JTokenType.Object:
			//		if (m_token[binder.Name].HasValues)
			//		{
			//			result = m_token[binder.Name];
			//		}
			//		else
			//			result = m_token[binder.Name].ToString();

			//		break;
			//	case JTokenType.Array:
			//		break;
			//	case JTokenType.String:
			//		m_token[binder.Name] = (string)value;
			//		break;
			//	default:
			//		m_token[binder.Name] = value;
			//		break;
			//}


			//m_token[binder.Name] = (string)value;
			return true;

			//return base.TrySetMember(binder, value);
		}

		public override IEnumerable<string> GetDynamicMemberNames()
		{
			return ((JObject)m_token).Properties().Select(p => p.Name.ToString()).ToList();
		}

		#endregion DynamicObject Overrides

		private void UpdateValue(string name, object value)
		{
			m_token[name] = (string)value;
		}

		#region Methods

		public void Remove(string Name)
		{
			m_token[Name].Parent.Remove();


		}

		public void SetDefaultValues(string value = null)
		{
			foreach (JProperty prop in m_token.Children())
			{
				switch (m_token[prop.Name].Type)
				{
					case JTokenType.Object:
						if (m_token[prop.Name].HasValues)
						{
							foreach (JProperty subproperty in m_token[prop.Name].Children())
								subproperty.Value = new JValue(value);
						}
						else
							prop.Value = new JValue(value);

						break;
					case JTokenType.Array:
						JArray ary = JArray.Parse(m_token[prop.Name].ToString());
						//if (ary.Children().Count() >= 1)
						for (int i = ary.Children().Count() - 1; i >= 1; i--)
							ary.RemoveAt(i);

						if (ary.Count() > 0)
							foreach (JProperty subprop in ary[0])
								subprop.Value = new JValue(value);

						m_token[prop.Name] = ary;
						break;
					default:
						prop.Value = new JValue(value);
						break;
				}
			}
		}

		public void SetDefaultValues(string property, string value = null)
		{
			foreach (JProperty prop in m_token[property])
			{
				switch (m_token[prop.Name].Type)
				{
					case JTokenType.Object:
						if (m_token[prop.Name].HasValues)
						{
							foreach (JProperty subProperty in m_token[prop.Name].Children())
							{
								SetDefaultValues(prop.Name, value);
							}
						}
						else
							prop.Value = new JValue(value);
						break;

					case JTokenType.Array:
						JArray ary = JArray.Parse(m_token[prop.Name].ToString());
						if (ary.Children().Count() >= 1)
							for (int i = ary.Children().Count(); i <= 1; i--)
								ary.RemoveAt(i);

						m_token[prop.Name] = ary;
						SetDefaultValues(prop.Name, value);

						break;
					default:
						prop.Value = new JValue(value);
						break;
				}




				prop.Value = new JValue(value);
			}
		}

		public void Update(string name, string value = null)
		{
			//switch (m_token[name].Type)
			//{
			//	case JTokenType.Object:
			//		if (m_token[name].HasValues)
			//		{
			//			foreach (JToken child in m_token[name].Children())
			//			{
			//				m_token[name].Children() = value;
			//			}
			//		}
			//		else
			//			result = m_token[name].ToString();

			//		break;
			//	case JTokenType.Array:
			//		m_token[name] = value;


			//		break;
			//	case JTokenType.String:
			//		result = m_token[name].ToString();
			//		break;
			//	default:
			//		result = m_token[name];
			//		break;
			//}



			if (((JObject)m_token).ContainsKey(name))
			{
				m_token[name] = value;
			}
		}

		#endregion Methods
	}
}
