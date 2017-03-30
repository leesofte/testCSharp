using System;
using System.Collections;
using System.Data;
using System.Xml;
using System.Web;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.InteropServices;


namespace NAT
{
    namespace INFRA
    {
        internal class Win32MapApi
        {
        }
        public class DevCache
        {
            private System.Threading.Mutex oMutex = new System.Threading.Mutex(false, "MmfUpdater");
            public DevCache()
                : base()
            {

            }


            private const string ObjectNamesMMF = "ObjectNamesMMF";
            /// <summary>
            /// create a MMF and serialize the object in to.
            /// </summary>
            /// <param name="InObject"></param>
            /// <param name="obectName"></param>
            /// <param name="ObjectSize"></param>
            private void WriteObjectToMMF(object InObject, string obectName, int ObjectSize)
            {
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(ms, InObject);

                oMutex.WaitOne();
                MemoryMappedFile map = new MemoryMappedFile(obectName + ".nat", MapProtection.PageReadWrite, MapAccess.FileMapAllAccess, ms.Length, obectName);
                MapViewStream stream = map.MapView(MapAccess.FileMapAllAccess, 0, (int)ms.Length, "");
                stream.Write(ms.GetBuffer(), 0, (int)ms.Length);
                stream.Flush();
                stream.Close();
                oMutex.ReleaseMutex();
            }


            /// <summary>
            /// this function will 
            /// 1) open exsisting (if not create) MMF that hold all the MMf names for  each object
            /// 2) look if aname allready exist
            /// 3) if exist 
            ///			-Delete the MMF
            ///			-create new MMF
            ///			-Enter the onject into
            ///		if not
            ///			-create new MMF
            ///			-Enter the onject into
            ///			-enter the new name and MMF name into MMF of object and MMF name
            /// </summary>
            /// <param name="objName"></param>
            /// <param name="inObject"></param>
            public void AddObject(string objName, object inObject, bool UpdateDomain)
            {
                MemoryMappedFile map = new MemoryMappedFile();
                System.Collections.Hashtable oFilesMap;
                System.IntPtr oAtom = System.IntPtr.Zero;
                string strIps = "";

                try
                {

                    if (!map.OpenEx(ObjectNamesMMF + ".nat", MapProtection.PageReadWrite, ObjectNamesMMF, MapAccess.FileMapAllAccess))
                    {
                        //Create MMF for the object and serialize it
                        WriteObjectToMMF(inObject, objName, 0);
                        //create hashtable
                        oFilesMap = new System.Collections.Hashtable();
                        //add object name and mmf name to hash
                        oFilesMap.Add(objName, objName);
                        //create main MMF
                        WriteObjectToMMF(oFilesMap, ObjectNamesMMF, 0);
                    }
                    else
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        Stream mmfStream = map.MapView(MapAccess.FileMapRead, 0, 0, "");
                        mmfStream.Position = 0;
                        oFilesMap = bf.Deserialize(mmfStream) as Hashtable;
                        long StartPosition = mmfStream.Position;

                        if (oFilesMap.ContainsKey(objName))
                        {
                            //name exist so we need to 
                            //	open the MMF of the existing and update it
                            MemoryMappedFile MemberMap = new MemoryMappedFile();
                            oMutex.WaitOne();
                            MemberMap.OpenEx(objName + ".nat", MapProtection.PageReadWrite, objName, MapAccess.FileMapAllAccess);   //(MapAccess.FileMapAllAccess ,objName);						
                            MapViewStream stream = MemberMap.MapView(MapAccess.FileMapAllAccess, 0, (int)0, "");
                            bf = new BinaryFormatter();
                            MemoryStream ms = new MemoryStream();
                            bf.Serialize(ms, inObject);
                            stream.Position = 0;
                            stream.Write(ms.GetBuffer(), 0, (int)ms.Length);
                            stream.Flush();
                            stream.Close();
                            oMutex.ReleaseMutex();
                        }
                        else
                        {
                            //name not apear so we nedd to
                            //	craete new MMF file and serialize
                            WriteObjectToMMF(inObject, objName, 0);
                            oMutex.WaitOne();
                            MapViewStream stream = map.MapView(MapAccess.FileMapAllAccess, 0, (int)0, "");
                            // update the main HashTable
                            oFilesMap.Add(objName, objName);
                            // serialize new Hash
                            bf = new BinaryFormatter();
                            MemoryStream ms = new MemoryStream();
                            bf.Serialize(ms, oFilesMap);
                            stream.Position = 0;
                            stream.Write(ms.GetBuffer(), 0, (int)ms.Length);
                            stream.Flush();
                            stream.Close();
                            oMutex.ReleaseMutex();
                        }
                    }

                }

                catch (Exception e)
                {
                    throw new Exception("Cannot Open File " + objName, e);
                }
                finally
                {
                    Win32MapApis.GlobalDeleteAtom(oAtom);
                }
            }


            /// <summary>
            /// GetObject return the object from MMF 
            /// </summary>
            /// <param name="objName"></param>
            /// <returns></returns>
            public object GetObject(string objName)
            {
                MemoryMappedFile map = new MemoryMappedFile();
                MemoryMappedFile mapOfName = new MemoryMappedFile();
                System.Collections.Hashtable oFilesMap;
                try
                {
                    oMutex.WaitOne();
                    if (!map.OpenEx(ObjectNamesMMF + ".NAT", MapProtection.PageReadOnly, ObjectNamesMMF, MapAccess.FileMapRead))
                        throw new Exception("No Desc FileFound");
                    BinaryFormatter bf = new BinaryFormatter();
                    Stream mmfStream = map.MapView(MapAccess.FileMapRead, 0, 0, "");
                    mmfStream.Position = 0;
                    //mmfStream.Close ();
                    oFilesMap = bf.Deserialize(mmfStream) as Hashtable;
                    long StartPosition = mmfStream.Position;
                    if (!oFilesMap.ContainsKey(objName))
                        throw new Exception("No Name Found");
                    if (!mapOfName.OpenEx(oFilesMap[objName].ToString() + ".NAT", MapProtection.PageReadOnly, oFilesMap[objName].ToString(), MapAccess.FileMapRead))
                        throw new Exception("No Name File Found");
                    mmfStream.Close();
                    mmfStream = null;


                    mmfStream = mapOfName.MapView(MapAccess.FileMapRead, 0, 0, oFilesMap[objName].ToString() + ".NAT");
                    mmfStream.Position = 0;

                    //mmfStream.SetLength (80000);
                    object oRV = bf.Deserialize(mmfStream) as object;
                    mmfStream.Close();
                    return oRV;
                }
                catch (Exception e)
                {
                    return null;
                    //throw new Exception("Cannot create File "+ objName,e);
                }
                finally
                {
                    oMutex.ReleaseMutex();
                }
            }
            public void RemoveObject(string ObjName)
            {
                //get the main mmf table and remove the key
                MemoryMappedFile map = new MemoryMappedFile();
                if (map.OpenEx(ObjectNamesMMF + ".nat", MapProtection.PageReadWrite, ObjectNamesMMF, MapAccess.FileMapAllAccess))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    MapViewStream mmfStream = map.MapView(MapAccess.FileMapRead, 0, 0, "");
                    mmfStream.Position = 0;
                    Hashtable oFilesMap = bf.Deserialize(mmfStream) as Hashtable;
                    oFilesMap.Remove(ObjName);
                    mmfStream.Close();
                    //update the main file
                    bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, oFilesMap);
                    MapViewStream stream = map.MapView(MapAccess.FileMapAllAccess, 0, (int)0, "");
                    stream.Position = 0;
                    stream.Write(ms.GetBuffer(), 0, (int)ms.Length);
                    stream.Flush();
                    stream.Close();
                    //delete the map of the object
                    MemoryMappedFile oMMf = new MemoryMappedFile();
                    if (oMMf.Open(MapAccess.FileMapAllAccess, ObjName))
                    {
                        oMMf.Close();
                        oMMf.Dispose();
                    }
                    if (System.IO.File.Exists(map.GetMMFDir() + ObjName + ".nat"))
                        System.IO.File.Delete(map.GetMMFDir() + ObjName + ".nat");
                }


            }
        }
    }
}