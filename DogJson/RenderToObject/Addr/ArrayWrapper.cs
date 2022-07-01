﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
    public unsafe class ArrayWrapper
    {
        int maxRank = 10;
        public ArrayWrapper(int Max_Rank = 10)
        {
            this.maxRank = Max_Rank;
            intPtr = Marshal.AllocHGlobal(100);
            array_lengths = (int*)intPtr.ToPointer();

        }
        ~ArrayWrapper()
        {
            Marshal.FreeHGlobal(intPtr);
        }

        IntPtr intPtr;
        int* array_lengths;
        int rank = 0;
        public int arraySize = 1;

        Dictionary<Type, ArrayWrapperItem> allArrayTypeHeadAddr = new Dictionary<Type, ArrayWrapperItem>();
        public void SetSize(int length)
        {
            array_lengths[rank] = length;
            ++rank;
            arraySize *= length;
            if (maxRank < rank)
            {
                maxRank = rank;
            }
        }
        //public void ResetSize()
        //{
        //    rank = 0;
        //    array_size = 1;
        //}
        public readonly static int array1StartOffcet = UnsafeOperation.PTR_COUNT * 2;


        public object CreateArrayOne(Type type, int length, out byte* startOffcet, out GCHandle gCHandle, out int itemSize)
        {
            ArrayWrapperItem arrayWrapperItem;
            if (allArrayTypeHeadAddr.TryGetValue(type, out arrayWrapperItem))
            {
            }
            else
            {
                allArrayTypeHeadAddr[type] = arrayWrapperItem = new ArrayWrapperItem(type, maxRank);
            }
            itemSize = arrayWrapperItem.typeSize;
            int arrayMsize = length * arrayWrapperItem.typeSize;
            object array = new byte[arrayMsize];
            gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(array);
            *p = arrayWrapperItem.heads[1];
            ++p;
            *p = (IntPtr)length;
            startOffcet = (byte*)GeneralTool.ObjectToVoid(array) + array1StartOffcet;
            return array;
        }

         public object CreateArray(Type type, out byte* startOffcet, out GCHandle gCHandle, 
             out int itemTypeSize)
        {
            ArrayWrapperItem arrayWrapperItem;
            if (allArrayTypeHeadAddr.TryGetValue(type, out arrayWrapperItem))
            {
                if (arrayWrapperItem.maxRank < maxRank)
                {
                    allArrayTypeHeadAddr[type] = arrayWrapperItem = new ArrayWrapperItem(type, maxRank);
                }
            }
            else
            {
                allArrayTypeHeadAddr[type] = arrayWrapperItem = new ArrayWrapperItem(type, maxRank);
            }
            itemTypeSize = arrayWrapperItem.typeSize;
            int offcet = (rank * 2 - 1) * 4;
            int arrayMsize = offcet + arraySize * itemTypeSize;
            object array = new byte[arrayMsize];
            gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);

            IntPtr* p = (IntPtr*)GeneralTool.ObjectToVoid(array);

            *p = arrayWrapperItem.heads[rank];
            ++p;

            *p = (IntPtr)arraySize; ++p;
            startOffcet = (byte*)GeneralTool.ObjectToVoid(array) + offcet + array1StartOffcet + 4;
            GeneralTool.Memcpy(p, array_lengths, rank * 4);
            rank = 0;
            arraySize = 1;
            return array;
        }

        public class ArrayWrapperItem
        {
            public ArrayWrapperItem(Type itemType, int maxRank)
            {
                this.itemType = itemType;
                this.maxRank = maxRank;
                this.typeSize = UnsafeOperation.SizeOfStack(itemType);
                heads = new IntPtr[maxRank + 1];
                heads[1] = *(IntPtr*)GeneralTool.ObjectToVoid(Array.CreateInstance(itemType, 0));
                for (int i = 2; i < maxRank + 1; i++)
                {
                    object objarr = Array.CreateInstance(itemType, new int[i]);
                    heads[i] = *(IntPtr*)GeneralTool.ObjectToVoid(objarr);
                }

            }
            public int maxRank;
            public Type itemType;
            public IntPtr[] heads;
            public int typeSize;
        }

    }

}