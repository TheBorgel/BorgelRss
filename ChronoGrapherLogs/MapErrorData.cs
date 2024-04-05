using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChronoGrapherLogs
{
	/// <summary>
	/// Single Map File
	/// </summary>
	public class MapFile
	{
		/// <summary>
		/// File name
		/// </summary>
		public String? Name { get; set; }

		/// <summary>
		/// File Data
		/// </summary>
		public String? Data { get; set; }
	}

	/// <summary>
	/// Map error report
	/// </summary>
	public class MapFileErrorReport
	{
		/// <summary>
		/// Map Name
		/// </summary>
		public String? User { get; set; }

		/// <summary>
		/// Map Name
		/// </summary>
		public String? MapName { get; set; }

		/// <summary>
		/// Map files
		/// </summary>
		public MapFile[]? Files { get; set; }
	}
}
