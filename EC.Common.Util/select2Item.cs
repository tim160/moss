using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EC.Common.Util
{
  /// <summary>
  /// 
  /// </summary>
  public class select2Item : IEquatable<select2Item>, IComparable<select2Item>
  {
    public string id { get; set; }
    public string text { get; set; }

    public select2Item()
    {
    }

    public select2Item(string text)
    {
      this.text = this.id = text;
    }

    public select2Item(string text, string id)
    {
      this.text = text;
      this.id = id;
    }

    public static List<select2Item> ItemsFactory(params string[] itemsText)
    {
      List<select2Item> r = new List<select2Item>();
      foreach (string s in itemsText)
      {
        r.Add(new select2Item(s));
      }
      return r;
    }

    int IComparable<select2Item>.CompareTo(select2Item other)
    {
      if (other == null)
        return 1;
      return this.text.CompareTo(other.text);
    }

    bool IEquatable<select2Item>.Equals(select2Item other)
    {
      if (other == null) return false;
      return (this.text.Equals(other.text) && this.id.Equals(other.id));
    }

    public static select2Item EmptyItem0 = new select2Item("", "0");
  }

  public class Select2ItemComparer2 : IEqualityComparer<select2Item>
  {

    public bool Equals(select2Item x, select2Item y)
    {
      return string.Compare(x.id, y.id, true) == 0 && string.Compare(x.text, y.text, true) == 0;
    }

    public int GetHashCode(select2Item obj)
    {
      return obj.id.GetHashCode() ^
          obj.text.GetHashCode();
    }
  }


  public class select2ItemGroup
  {
    public string title { get; set; }
    public List<select2Item> children { get; set; }

    public select2ItemGroup()
    {
      children = new List<select2Item>();
    }
  }

}
