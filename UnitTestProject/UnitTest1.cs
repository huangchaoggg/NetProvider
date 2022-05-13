using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetProvider.Network;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            
            //HashSet<int> vs=new HashSet<int>();
            //var c = 1 ^ 2;
            string sss = HttpWebHelper.PathCombine("https://docs.microsoft.com", "/zh-cn/dotnet/breadcrumb/toc.json");
            Console.WriteLine(sss);
        }
        [TestMethod]
        public void TestMedian()
        {
            int[] n1 = new int[] { 1,2,2,1};
            int[] n2 = new int[] { 2, 2};
            var val = Intersect(n1, n2);
            Console.WriteLine(val);
        }
        public int[] Intersect(int[] nums1, int[] nums2)
        {
            int count = nums1.Length > nums2.Length ? nums1.Length : nums2.Length;
            List<int> jf = new List<int>(count);
            foreach (var v in nums1)
            {
                foreach (var n in nums2)
                {
                    var yh = v ^ n;
                    if (yh == 0)
                    {
                        jf.Add(v);
                        break;
                    }
                }
            }
            return jf.ToArray();
        }

        [TestMethod]
        public void TestPlusOne()
        {
            int[] n1 = new int[] { 5,6,1,3 };
            var val = MaxRotateFunction(n1);
            Console.WriteLine(val);
        }
        public int MaxRotateFunction(int[] nums)
        {
            int[] sums = new int[nums.Length];
            int n = 0;
            LinkedList<int> kd = new LinkedList<int>(nums);
            foreach(var a in kd)
            {
            }
            while (n < nums.Length)
            {
                if (n > 0)
                {
                    kd.AddFirst(kd.Last.Value);
                    kd.RemoveLast();
                }
                int[] ks = kd.ToArray();
                if (ks.Length == 1) return sums[n] = 0;
                if (ks.Length == 2) return sums[n] = ks[1];
                ks[0] = 0;
                for (int i = 2; i < ks.Length; i++)
                {
                    ks[i] = ks[i] * i;
                }
                sums[n] = ks.Sum();
                n++;
            }
            return sums.Max();
        }
        //public int[] PlusOne(int[] digits)
        //{
        //    LinkedList<int> lk;
        //    var str = (int.Parse(string.Concat(digits)) + 1).ToString();
        //    var strArr = str.ToArray();
        //    var ls = new List<int>();
        //    foreach (var sa in strArr)
        //    {
        //        ls.Add(int.Parse(sa+""));
        //    }
        //    return ls.ToArray();
        //}
        //public double FindMedianSortedArrays(int[] nums1, int[] nums2)
        //{
        //    int[] nums = nums1.Concat(nums2).ToArray();
        //    if (nums.Length == 1) return nums[0];
        //    bool isZ = nums.Length % 2 == 0;
        //    int zws = (isZ ? (nums.Length / 2 + 1) : (nums.Length - 1) / 2 + 1);
        //    int i = 1, k = 0;
        //    while (k < zws)
        //    {
        //        if (nums[k] > nums[i])
        //        {
        //            int n = nums[i];
        //            nums[i] = nums[k];
        //            nums[k] = n;
        //        }
        //        else
        //        {

        //            i++;

        //            if (i>= nums.Length)
        //            {
        //                k++;
        //                i = k + 1;
        //                if (i >= nums.Length) break;
        //            }
        //            continue;
        //        }

        //    }
        //    //for (int i = 0; i < zws; i++)
        //    //{
        //    //    for (int k = i; k < nums.Length; k++)
        //    //        if (nums[i] > nums[k])
        //    //        {
        //    //            int n = nums[i];
        //    //            nums[i] = nums[k];
        //    //            nums[k] = n;
        //    //        }
        //    //}
        //    if (isZ)
        //    {
        //        double n = (nums[zws - 1] + nums[zws - 2]) / 2d;
        //        return n;
        //    }
        //    else
        //    {
        //        return nums[zws - 1];
        //    }

        //}
    }
}
