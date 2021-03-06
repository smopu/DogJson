using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace DogJson
{
    public unsafe interface IJsonRenderToObject
    {
        object CreateObject(JsonReader jsonRender, char* startChar, int length);
    }

    public unsafe interface IJsonWriterToObject
    {
        List<JsonWriteValue> ReadObject(object data);
    }
}
