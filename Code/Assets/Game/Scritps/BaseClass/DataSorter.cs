
using System.Collections;

public class TurnSorter : IComparer
{
	public int Compare(System.Object x , System.Object y)
	{
        int sort = 0;
		
		Hashtable ht1 = (Hashtable)x;
        Hashtable ht2 = (Hashtable)y;
		
		float f1 =float.Parse((string)ht1["value"]);
		float f2=float.Parse((string)ht2["value"]);
		
		if(f1 == f2) sort = 0;
		else if(f1 < f2) sort = 1;
		else if(f1 > f2) sort = -1;
		
		return sort;
    } 
}


public class ItemNameSorter : IComparer
{
	public int Compare(System.Object x , System.Object y)
	{
		int i1 = (int)x;
		int i2 = (int)y;
		string n1 = DataHolder.Items().GetName(i1);
		string n2 = DataHolder.Items().GetName(i2);
		return n1.CompareTo(n2);
	}
}