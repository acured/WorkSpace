from TCPCameraAdapter import TCPCameraAdapter

adapter = TCPCameraAdapter(5555)
adapter.ConnStart()
adapter.DoListen()
