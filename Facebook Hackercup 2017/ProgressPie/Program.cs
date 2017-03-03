using System;
using System.IO;
using System.Linq;

namespace Progress_Pie
{
	class Program
	{
		public static void Main(string[] args)
		{
			//Console.SetIn(new StreamReader("input.txt"));
			int T = int.Parse(Console.ReadLine());
			var eps = 0.0000001;
			
			// center of circle
			var cx = 50.0;
			var cy = 50.0;
			var r = 50.0;
					
			for (int i=0; i<T; ++i)
			{
				var tok = Console.ReadLine().Split(' ').Select(t=>double.Parse(t)).ToArray();
				var p = tok[0];
				var px = tok[1];
				var py = tok[2];
				var result = "white";
				
				if (((px-cx)*(px-cx) + (py-cy)*(py-cy))<=r*r)
				if (p > eps)
				{				
					// Detect point quadrant
					var pq = 1;
					if (px < cx)
					{
						pq = 4;
						if (py < cy)
						{
							pq = 3;
						}
					}
					else
					{
						if (py < cy)
						{
							pq = 2;
						}
					}
					
					var angle = p*360.0/100.0;					
					var alpha = Math.PI/2 + 2*Math.PI*(1 - p/100.0);
					
					var ax = cx+r*Math.Cos(alpha);
					var ay = cy+r*Math.Sin(alpha);
					
					var aq = 1;
					if (angle > 90) aq = 2;
					if (angle > 180) aq = 3;
					if (angle > 270) aq = 4;
					
					if (pq < aq)
					{
						result = "black";
					}
					else if (pq == aq)
					{
						double vx,vy;
						
						if (pq == 1)
						{
							 vx = cx;
							 vy = 100.0;
						}
						else if (pq  == 2)
						{
							vx = 100.0;
							vy = cy;							
						}
						else if (pq == 3)
						{
							vx = cx;
							vy = 0.0;
						}
						else
						{
							vx = 0.0;
							vy = cy;
						}
						
						var vx_1 = vx - cx;
						var vy_1 = vy - cy;
						
						var tx_1 = px - cx;
						var ty_1 = py - cy;
						
						var vx_2 = ax - cx;
						var vy_2 = ay - cy;
						
						var cos_vt = (vx_1*tx_1 + vy_1*ty_1)/(Math.Sqrt(vx_1*vx_1 + vy_1*vy_1)*Math.Sqrt(tx_1*tx_1 + ty_1*ty_1));
						var cos_vv = (vx_1*vx_2 + vy_1*vy_2)/(Math.Sqrt(vx_1*vx_1 + vy_1*vy_1)*Math.Sqrt(vx_2*vx_2 + vy_2*vy_2));
						
						if (cos_vv <= cos_vt)
						{
							result = "black";
						}
					}
				}
				
				Console.Write(String.Format("Case #{0}: {1}", i+1, result));
				Console.Write('\n');
			}
		}
	}
}