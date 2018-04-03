using System;
using System.Text;

// https://www.hackerrank.com/challenges/morgan-and-a-string/problem
class MorganAndString
{
    static string morganAndString(string a, string b) {
    var r = new StringBuilder();
    int i = 0;
    int j = 0;
    while(true){
        if (i<a.Length && j<b.Length)
        {
            if (a[i]<b[j])
            {
                r.Append(a[i]); i++;
            }
            else if (b[j]<a[i])
            {
                r.Append(b[j]); j++;
            }
            else
            {
                int k = 0;
                while ((i+k<a.Length) && (j+k<b.Length) && (a[i+k]==b[j+k])) ++k;
             
                if ((j+k>=b.Length) && (i+k>=a.Length))
                {
                    while(i<a.Length && a[i]==b[j])
                    {
                       r.Append(a[i]); i++;
                    }
                }
                else if (j+k>=b.Length)
                {
                    for (int g=0; g<k; ++g)
                    {
                      r.Append(a[i]); i++;
                    }
                }
                else if (i+k>=a.Length)
                {
                    for (int g=0; g<k; ++g)
                    {
                      r.Append(b[j]); j++;
                    }
                }
                else if (a[i+k]<b[j+k])
                {
                   r.Append(a[i]); i++;
                }
                else
                {
                   r.Append(b[j]); j++;
                } 
            }
        }
        else if (i<a.Length)
        {
            r.Append(a[i]); i++;
        }
        else if (j<b.Length)
        {
            r.Append(b[j]); j++;
        }
        else
        {
           return r.ToString();    
        }
     }         
    }

    public static void Main(string[] args)
    {
     var t = int.Parse(Console.ReadLine());
     for (int i=0; i<t; ++i){
       var a = Console.ReadLine();
       var b = Console.ReadLine();
       Console.WriteLine(morganAndString(a,b));
     }
    }
}