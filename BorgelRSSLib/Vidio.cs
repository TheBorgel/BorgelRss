using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BorgelRSSLib
{
	public class Vidio
	{
		public DateTime Published { get; set; }
		public String Link { get; set; }
		public String Author { get; set; }
		public String Title { get; set; }
		public String Thumbnail { get; set; }
		public String Length { get; set; }

		private XmlNode m_node = null;

		public Vidio( XmlNode i_node )
		{
			m_node = i_node;

			Published = DateTime.Parse( i_node[ "published" ].InnerText );
			Link = m_node[ "link" ].GetAttribute( "href" );
			Author = m_node[ "author" ][ "name" ].InnerText;
			Thumbnail = m_node[ "media:group" ][ "media:thumbnail" ].GetAttribute( "url" );
			Title = m_node[ "title" ].InnerText;
		}

		public void Init()
		{
			String t_page = LenghtFinder.Find( Link );
			Length = LenghtFinder.CreateTimeString( t_page );
		}
	}
}
