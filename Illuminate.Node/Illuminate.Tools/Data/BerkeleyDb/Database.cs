using System;
using System.Collections.Generic;
using System.Text;
using BerkeleyDb;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Illuminate.Tools.Data.Berkeley
{
	public class Database
	{
		protected BerkeleyDb.Db db;
		protected DbFile dbFile;

		public Database(string fileName)
		{
			if (!Directory.Exists("data"))
				Directory.CreateDirectory("data");

			fileName = "data/" + fileName;

			db = new Db(DbCreateFlags.None);
			db.CacheSize = new CacheSize(0, 10240, 0);
			dbFile = db.Open(null, fileName, null, DbType.BTree, Db.OpenFlags.Create | Db.OpenFlags.ThreadSafe, 0);
		}

		public bool Set(string key, object data)
		{
			BinaryFormatter bf = new BinaryFormatter();
			MemoryStream ms = new MemoryStream();
			bf.Serialize(ms, data);


			BerkeleyDb.DbEntry DBkey = DbEntry.InOut(Encoding.UTF8.GetBytes(key));
			BerkeleyDb.DbEntry DBdata = DbEntry.InOut(ms.ToArray());

			WriteStatus status = dbFile.Put(null, ref DBkey, ref DBdata, DbFile.WriteFlags.None);
			dbFile.Sync();

			if (status == WriteStatus.Success)
				return true;

			return false;
		}

		public void BigSet(object[] param)
		{
			int cnt = 0;

			try
			{
				while (cnt < param.Length)
				{
					int keyIdx = cnt;
					int valIdx = cnt + 1;

					BinaryFormatter bf = new BinaryFormatter();
					MemoryStream ms = new MemoryStream();
					bf.Serialize(ms, param[valIdx]);

					BerkeleyDb.DbEntry DBkey = DbEntry.InOut(Encoding.UTF8.GetBytes(param[keyIdx].ToString()));
					BerkeleyDb.DbEntry DBdata = DbEntry.InOut(ms.ToArray());

					WriteStatus status = dbFile.Put(null, ref DBkey, ref DBdata, DbFile.WriteFlags.None);

					cnt = cnt + 2;
				}
			}
			catch (Exception e)
			{
			}
		}

		public object Get(string key)
		{
			DbEntry DBkey = DbEntry.InOut(Encoding.UTF8.GetBytes(key));
			DbEntry DBdata = DbEntry.InOut(new byte[1024]);
			ReadStatus status = dbFile.Get(null, ref DBkey, ref DBdata, DbFile.ReadFlags.None);

			if (status == ReadStatus.Success)
			{
				MemoryStream memStream = new MemoryStream();
				BinaryFormatter binForm = new BinaryFormatter();
				memStream.Write(DBdata.Buffer, 0, DBdata.Buffer.Length);
				memStream.Seek(0, SeekOrigin.Begin);
				
				object obj = (Object) binForm.Deserialize(memStream);
				return obj;
			}

			return string.Empty;
		}

		public object[] BigGet(object[] keys)
		{
			object[] values = new object[keys.Length];

			for (int i = 0; i < values.Length; i++)
			{
				values[i] = Get(keys[i].ToString());
			}

			return values;
		}
	}
}
