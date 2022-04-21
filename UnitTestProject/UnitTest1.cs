using System;
using System.Linq;

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
            string sss = HttpWebHelper.PathCombine("https://docs.microsoft.com", "/zh-cn/dotnet/breadcrumb/toc.json");
            Console.WriteLine(sss);
        }
        //[TestMethod]
        //public void TestMedian()
        //{
        //    int[] n1 = new int[] {  };
        //    int[] n2 = new int[] { 2,3 };
        //    double val = FindMedianSortedArrays(n1,n2);
        //    Console.WriteLine(val);
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
