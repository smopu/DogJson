﻿//#define NEW_Dictionary
using DogJson.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson
{
#if NEW_Dictionary
    public struct DictionaryKV<K, V>
    {
        public K k;
        public V v;
    }

    [Collection(typeof(Dictionary<,>), true)]
    public class CollectionArrayDictionary<K, V> : CollectionArrayBase<Dictionary<K, V>, DictionaryKV<K, V>[]>
    {
        TypeCode typeCodeK;
        TypeCode typeCodeV;
        public CollectionArrayDictionary()
        {
            typeCodeK = Type.GetTypeCode(typeof(K));
            typeCodeV = Type.GetTypeCode(typeof(V));
        }
        public override bool IsRef()
        {
            return false;
        }
        protected override void Add(DictionaryKV<K, V>[] dict, int index, object value)
        {
            if (index % 2 == 0)
            {
                dict[index / 2].k = (K)value;
            }
            else
            {
                dict[index / 2].v = (V)value;
            }
        }
        protected override DictionaryKV<K, V>[] CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
        {
            return new DictionaryKV<K, V>[arrayCount / 2];
        }
        protected override Dictionary<K, V> End(DictionaryKV<K, V>[] obj)
        {
            Dictionary<K, V> dict = new Dictionary<K, V>();
            ; for (int i = 0; i < obj.Length; i++)
            {
                dict[obj[i].k] = obj[i].v;
            }
            return dict;
        }
        public override Type GetItemType(int index)
        {
            if (index % 2 == 0)
            {
                return typeof(K);
            }
            return typeof(V);
        }

        protected override unsafe void AddValue(DictionaryKV<K, V>[] obj, int index, char* str, JsonValue* value)
        {
            if (index % 2 == 0)
            {
                obj[index / 2].k = (K)GetValue(typeCodeK, str, value);
            }
            else
            {
                obj[index / 2].v = (V)GetValue(typeCodeK, str, value);
            }
        }
    }
#else

    //[CollectionRead(typeof(DictionaryKV<,>), true)]
    //public unsafe class CollectionArrayDictionaryKV<K, V> : CollectionArrayBase<DictionaryKV<K, V>, DictionaryKV<K, V>>
    //{
    //    public override bool IsRef()
    //    {
    //        return false;
    //    }
    //    TypeCode typeCodeK;
    //    TypeCode typeCodeV;
    //    public CollectionArrayDictionaryKV()
    //    {
    //        typeCodeK = Type.GetTypeCode(typeof(K));
    //        typeCodeV = Type.GetTypeCode(typeof(V));
    //    }
    //    protected override void Add(DictionaryKV<K, V> kv, int index, object value, ReadCollectionProxy proxy)
    //    {
    //        if (index == 0)
    //        {
    //            kv.k = (K)value;
    //        }
    //        else
    //        {
    //            kv.v = (V)value;
    //        }
    //    }

    //    protected override void AddValue(DictionaryKV<K, V> kv, int index, char* str, JsonValue* value, ReadCollectionProxy proxy)
    //    {
    //        TypeCode typeCode;
    //        if (index == 0)
    //        {
    //            typeCode = typeCodeK;
    //        }
    //        else
    //        {
    //            typeCode = typeCodeV;
    //        }

    //        object set_value = proxy.callGetValue(typeCode, str, value);

    //        if (index == 0)
    //        {
    //            kv.k = (K)set_value;
    //        }
    //        else
    //        {
    //            kv.v = (V)set_value;
    //        }
    //    }

    //    protected override DictionaryKV<K, V> CreateArray(int arrayCount, object parent, Type arrayType, Type parentType)
    //    {
    //        return new DictionaryKV<K, V>();
    //    }
    //    public override Type GetItemType(int index)
    //    {
    //        if (index == 0)
    //        {
    //            return typeof(K);
    //        }
    //        return typeof(V);
    //    }

    //    protected override DictionaryKV<K, V> End(DictionaryKV<K, V> obj)
    //    {
    //        return obj;
    //    }
    //}

    public class DictionaryKV<K, V>
    {
        public K k;
        public V v;
    }

    [CollectionRead(typeof(DictionString<,>), false)]
    public unsafe class CollectionArrayDictionString<V> : CollectionObjectBase<Dictionary<string, V>, Dictionary<string, V>>
    //public unsafe class BoxCollection<T> : CollectionObjectBase<T, Box<T>>
    {
        TypeCode typeCode;
        public CollectionArrayDictionString()
        {
            typeCode = Type.GetTypeCode(typeof(V));
        }

        public override unsafe Type GetItemType(JsonObject* bridge)
        {
            return typeof(V);
        }

        public override bool IsRef()
        {
            return false;
        }

        protected override unsafe void Add(Dictionary<string, V> obj, char* key, int keyLength, object value, ReadCollectionProxy proxy)
        {
            obj[new string(key, 0, keyLength)] = (V)value;
        }

        protected override unsafe void AddValue(Dictionary<string, V> obj, char* key, int keyLength, char* str, JsonValue* value, ReadCollectionProxy proxy)
        {
            obj[new string(key, 0, keyLength)] = (V)proxy.callGetValue(typeCode, str, value);
        }

        protected override unsafe Dictionary<string, V> CreateObject(JsonObject* obj, object parent, Type objectType, Type parentType)
        {
            return new Dictionary<string, V>();
        }

        protected override Dictionary<string, V> End(Dictionary<string, V> obj)
        {
            return obj;
        }

    }




    [ReadCollection(typeof(DictionaryKV<,>), true)]
    public unsafe class CollectionArrayDictionaryKV<K, V> : CreateTaget<ReadCollectionLink>
    {
        TypeCode typeCodeK;
        TypeCode typeCodeV;
        public CollectionArrayDictionaryKV()
        {
            typeCodeK = Type.GetTypeCode(typeof(K));
            typeCodeV = Type.GetTypeCode(typeof(V));
        }

        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isRef = true;

            Action<DictionaryKV<K, V>, object, ReadCollectionLink.Add_Args> ac = Add;
            read.addDelegate = ac;
            read.create = Create_; 
            Action<DictionaryKV<K, V>, ReadCollectionLink.AddValue_Args> ac2 = AddValue;
            read.addValueDelegate = ac2;
            read.getItemType = GetItemType;
            return read;
        }

        static void Add(DictionaryKV<K, V> kv, object value, ReadCollectionLink.Add_Args arg)
        {
            if (arg.bridge->arrayIndex == 0)
            {
                kv.k = (K)value;
            }
            else
            {
                kv.v = (V)value;
            }
        }

        void AddValue(DictionaryKV<K, V> kv, ReadCollectionLink.AddValue_Args arg)
        {
            int index = arg.value->arrayIndex;
            TypeCode typeCode;
            if (index == 0)
            {
                typeCode = typeCodeK;
            }
            else
            {
                typeCode = typeCodeV;
            }

            object set_value = arg.callGetValue(typeCode, arg.str, arg.value);

            if (index == 0)
            {
                kv.k = (K)set_value;
            }
            else
            {
                kv.v = (V)set_value;
            }
        }
        void Create_(out object obj, out void* dataStart,  out object temp, ReadCollectionLink.Create_Args arg)
        {
            obj = new DictionaryKV<K, V>();
            temp = null;
            dataStart = ((IntPtr*)GeneralTool.ObjectToVoid(obj)) + 1;
        }

        Type GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            int index = arg.bridge->arrayIndex;
            if (index == 0)
            {
                return typeof(K);
            }
            return typeof(V);
        }
    }


    [ReadCollection(typeof(Dictionary<,>), true)]
    public unsafe class DictionaryReader<K, V> : CreateTaget<ReadCollectionLink>
    {
        public ReadCollectionLink Create()
        {
            ReadCollectionLink read = new ReadCollectionLink();
            read.isRef = true;

            Action<Dictionary<K, V>, DictionaryKV<K, V>, ReadCollectionLink.Add_Args> ac = Add;
            read.addDelegate = ac;
            read.create = Create_;
            read.getItemType = GetItemType;
            return read;
        }

        void Add(Dictionary<K, V> dict, DictionaryKV<K, V> kv, ReadCollectionLink.Add_Args arg)
        {
            dict[kv.k] = kv.v;
        }

        void Create_(out object obj, out void* dataStart,  out object temp, ReadCollectionLink.Create_Args arg)
        {
            obj = new Dictionary<K, V>(arg.bridge->arrayCount);
            temp = null;
            dataStart = ((IntPtr*)GeneralTool.ObjectToVoid(obj)) + 1;
        }

        Type GetItemType(ReadCollectionLink.GetItemType_Args arg)
        {
            return typeof(DictionaryKV<K, V>);
        }
    }


#endif


}
