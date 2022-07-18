﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogJson.Collection.ArrayCollection
{
    [CollectionWriteAttribute(typeof(List<>))]
    [CollectionWriteAttribute(typeof(Queue<>))]
    [CollectionWriteAttribute(typeof(HashSet<>))]
    [CollectionWriteAttribute(typeof(LinkedList<>))] 
    public unsafe class WriterEnumerable<T> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType() { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = (IEnumerable<T>)obj;
            foreach (var item in collection)
            {
                yield return new KeyValueStruct() { value = item, type = typeof(T) };
            }
        }
    }


    [CollectionWriteAttribute(typeof(Stack<>))]
    public unsafe class WriterStack<T> : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType() { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var collection = ((Stack<T>)obj).ToArray();
            for (int i = collection.Length - 1; i >= 0; i--)
            {
                yield return new KeyValueStruct() { value = collection[i], type = typeof(T) };
            }
        }
    }


    [CollectionWriteAttribute(typeof(EnumWrapper))]
    public unsafe class WriterEnum : IWriterCollectionObject
    {
        public JsonWriteType GetWriteType() { return JsonWriteType.Array; }
        public IEnumerable<KeyValueStruct> GetValue(object obj)
        {
            var inEnum = ((EnumWrapper)obj).inEnum;
            var type = inEnum.GetType();
            EnumTypeWrap enumTypeWrap = CollectionManager.GetEnumTypeWrap(type);
            long source;
            List<string> vs = enumTypeWrap.GetValue(inEnum, out source);
            if (vs.Count == 0)
            {
                yield return new KeyValueStruct() { value = source, type = typeof(long) };
            }
            foreach (var item in vs)
            {
                yield return new KeyValueStruct() { value = item, type = typeof(string) };
            }

        }


    }









}