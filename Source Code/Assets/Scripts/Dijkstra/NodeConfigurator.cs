//============================================================
// Node Configurator class : digunakan untuk set node dan path
//============================================================
// basic unity package
using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class NodeConfigurator : MonoBehaviour
{
	//define jumlah node
	public int NumNodes;
	//define node transform -wiring-
	public Transform[] Nodes; 
	//boolean untuk menentukan antar node mana saja yang terkoneksi
	public bool[,]  connectionExists;
	//nilai jarak antar node
	float[,]  distances;
	//array komponen line
	public LineRenderer[] line;
	//line holder untuk menyimpan line renderer
	public GameObject[] lineHolder;
	//variable untuk menyimpan jumlah edges / garis
	int NumEdges;
	//define start dan end
	public int a = 0, b = 2;
	//define Djikstra class
	public Dijkstra dijkstra;
	// material untuk penanda garis
	public Material lineMaterial, highlightMaterial, traveledMaterial;
	//komponen text untuk log
	public Text debugText;
	//variable path untuk menyimpan array step dari start ke end
	int[] path;
	// variable untuk menyimpan jarak tempuh dari start -> end
	float distanceTraveled;
	// string untuk menyimpan text path step
	string pathString = "";

	//----------- dipanggil ketika tombol 'run' dijalankan
	public void RunPathfinder()
	{
		//isi path[] dengan menjalankan method RunDijksta()
		path = runDijkstra ();
		//tandai path yang harus ditempuh dengan memanggil method markPath
		StartCoroutine(markPath (path));
		//dapatkan total jarak tempuh dari start -> end
		distanceTraveled = getTotalDistance (path);
		//dapatkan text path tempuh
		pathString = getPathString(path);
		//tampilkan dalam log
		debugText.text += "\n<color=#800000>=== Summary ===</color>";
		debugText.text += "\nDistance to end : " + distanceTraveled.ToString("F2");
		debugText.text +=  "\n" + pathString;
	}	

	//----------- dipanggil ketika scene pertama kali di play
	void Start () 
	{
		//pastikan jumlah nodes = jumah objek node
		NumNodes = Nodes.Length;
		//tampilkan di log
		debugText.text = "Jumlah Node = "+NumNodes;
		//set objek nodes
		initializeNodes();	
		//set garis
		initializeLines();
	}

	//-------- dipanggil realtime tiap frame
	void Update () 
	{
		// koneksikan tiap node yang sudah di set
		connectNodes();
		// Update warna Start Node
		Nodes [a].GetComponent<Renderer>().material.color = Color.red;
		// rubah text menjadi 'start'
		var CurrentCubeText = Nodes [a].Find("New Text").GetComponent(typeof(TextMesh)) as TextMesh;
		CurrentCubeText.text = "Start";
		CurrentCubeText.fontSize = 17;
		// Update warna end node
		Nodes [b].GetComponent<Renderer>().material.color = Color.red;
		// rubah text menjadi 'end'
		CurrentCubeText = Nodes [b].Find("New Text").GetComponent(typeof(TextMesh)) as TextMesh;
		CurrentCubeText.text = "End";
		CurrentCubeText.fontSize = 17;
	}
	
	//---------- method untuk set Node
	void initializeNodes()
	{	
		//init size connectionExists
		connectionExists = new bool[NumNodes,NumNodes];
		//init array distances
		distances = new float[NumNodes,NumNodes];
		// cek tiap node
		for (int i = 0; i < NumNodes; i++)
		{
			//pastikan textberdasarkan nomor nodes
			var CurrentCubeText = Nodes[i].Find("New Text").GetComponent(typeof(TextMesh)) as TextMesh;
			CurrentCubeText.text = ""+i;
			//ubah juga nama game objeknya
			Nodes[i].gameObject.name = i.ToString();
		}
		//jumlah garis diisi dengan berapa jumlah antar nodes yang terkoneksi
		NumEdges = determineConnections();
		//tampilkan di log jumlah garis
		debugText.text += "\nJumlah Path : "+NumEdges;
	}
	
	//------------- method untuk mendefinisikan garis antar nodes
	void initializeLines()
	{
		//define size variable line berasarkan jumlah garis
		line = new LineRenderer[NumEdges];
		//... negitu juga holdernya
		lineHolder = new GameObject[NumEdges];
		//untuk setiap garis / edge
		for (int i = 0; i < NumEdges; i++)
		{
			//define masing-masing garis
			line[i] = new LineRenderer();
			//... juga holder nya
			lineHolder[i] = new GameObject();
			//tambahkan komonen line ke objek holder
			line[i] = lineHolder[i].AddComponent<LineRenderer>();
			//set jumlah titik nya
			line[i].SetVertexCount(2);
			//set width garis
			line[i].SetWidth(.3F, .3F);
			//set material line
			line[i].material = lineMaterial;
			//enable line
			line[i].enabled = true;
		}
	}	

	//---------- method untuk mengkoneksikan node (visualisasi)
	void connectNodes()
	{
		// buat variable untuk menentukan node yang di cek dan neighbor nya
		Vector3 nodeToCheck, neighbor;
		//variable menentukan line mana yang sedang dipakai
		int currentLine = 0;		
		// untuk setiap jumlah node, cek node to check
		for (int i = 0; i < NumNodes; i++)
		{
			// node utk di cek = objek node ke 'i'
			nodeToCheck = Nodes [i].gameObject.transform.position;
			// untuk setiap jumlah node, check neighbor
			for (int j = 0; j < NumNodes; j++)
			{
				// neighbor = objek node ke 'j'
				neighbor = Nodes [j].gameObject.transform.position;
				//cek apakah current dan neighbor terkoneksi
				if (connectionExists[i,j])
				{
					//jika ya, buat garis dari current ke neighbor
					line[currentLine].SetPosition (0, nodeToCheck);
					line[currentLine].SetPosition (1, neighbor);
					//rubah nama objek holder menjadi sesuai garis
					lineHolder[currentLine].name = "Line from " + i + " to " + j;
					//update variable currentLine
					currentLine++;
				} 
			}
		}
	}
	
	
	int determineConnections()
	{
		//----------------------TO-DO : Buat menjadi loop
		// variable utk menentukan jarak
		float distance;
		// variable vector 3 utk menentukan current dan neighbor
		Vector3 nodeToCheck, neighbor;
		// conter berfungsi untuk num of edges
		int counter = 0; 
		// --------------------Buat koneksi Node 0 -> 1
		// node posisi awal
		nodeToCheck = Nodes [0].gameObject.transform.position;
		// node posisi neighbor
		neighbor = Nodes [1].gameObject.transform.position;
		// hitung distance-nya
		distance = Vector3.Distance(nodeToCheck, neighbor);
		// tampilkan ke log
		debugText.text += "\nDistance : "+0+" > "+1+" = "+distance;
		// declare koneksi current dan neighbor
		declareConnection(0,1,distance);	
		// update counter
		counter++;
		// --------------------Buat koneksi Node 0 -> 2
		nodeToCheck = Nodes [0].gameObject.transform.position;
		neighbor = Nodes [2].gameObject.transform.position;
		distance = Vector3.Distance(nodeToCheck, neighbor);
		debugText.text += "\nDistance : "+0+" > "+2+" = "+distance;
		declareConnection(0,2,distance);					
		counter++;
		// --------------------Buat koneksi Node 2 -> 3
		nodeToCheck = Nodes [2].gameObject.transform.position;
		neighbor = Nodes [3].gameObject.transform.position;
		distance = Vector3.Distance(nodeToCheck, neighbor);
		debugText.text += "\nDistance : "+2+" > "+3+" = "+distance;
		declareConnection(2,3,distance);					
		counter++;
		// --------------------Buat koneksi Node 2 -> 4
		nodeToCheck = Nodes [2].gameObject.transform.position;
		neighbor = Nodes [4].gameObject.transform.position;
		distance = Vector3.Distance(nodeToCheck, neighbor);
		debugText.text += "\nDistance : "+2+" > "+4+" = "+distance;
		declareConnection(2,4,distance);
		counter++;
		// --------------------Buat koneksi Node 1 -> 5
		nodeToCheck = Nodes [1].gameObject.transform.position;
		neighbor = Nodes [5].gameObject.transform.position;
		distance = Vector3.Distance(nodeToCheck, neighbor);
		debugText.text += "\nDistance : "+1+" > "+5+" = "+distance;
		declareConnection(1,5,distance);					
		counter++;
		// --------------------Buat koneksi Node 4 -> 6
		nodeToCheck = Nodes [4].gameObject.transform.position;
		neighbor = Nodes [6].gameObject.transform.position;
		distance = Vector3.Distance(nodeToCheck, neighbor);
		debugText.text += "\nDistance : "+4+" > "+6+" = "+distance;
		declareConnection(4,6,distance);					
		counter++;
		// --------------------Buat koneksi Node 5 -> 5
		nodeToCheck = Nodes [5].gameObject.transform.position;
		neighbor = Nodes [6].gameObject.transform.position;
		distance = Vector3.Distance(nodeToCheck, neighbor);
		debugText.text += "\nDistance : "+5+" > "+6+" = "+distance;
		declareConnection(5,6,distance);					
		counter++;
		// return nilai counter menjadi nilai jumlah edges
		return counter;
	}
	
	//------------- method untuk menentukan node mana saja yang terhubung
	void declareConnection(int i, int j, float distance)
	{
		// buat [i , j] = true
		connectionExists[i,j] = true;
		// isi jarak i _ j
		distances[i,j] = distance;
	}
	
	//------------- method untuk menandai path yang sudah di generate
	IEnumerator markPath(int[] path){
		//untuk setiap objek holder ...
		foreach (GameObject o in lineHolder) {
			//... non-aktifkan holder
			//o.SetActive(false);
		}
		//buat variable current dan neighbor
		Vector3 nodeToCheck, neighbor;
		//tampilkan di log
		debugText.text += "\n<color=#800000>Tandai Path</color>";
		//untuk setiap nilai path
		for (int i = 0; i < path.Length; i++) {
			//tampilkan di log
			debugText.text += "\nnode to check : "+path[i];
			//current = node dengan nilai path 'i'
			nodeToCheck = Nodes[path[i]].gameObject.transform.position;
			//untuk setiap nilai di path
			for(int j = 0; j < path.Length; j++){
				//neighbor = node dengan nilai path 'j'
				neighbor = Nodes[path[j]].gameObject.transform.position;
				//tampilkan dalam log
				debugText.text += "\ncek : "+path[i]+" + "+path[j];
				//jika current dan neighbor terhubung
				if (connectionExists[path[i],path[j]])
				{
					// buat string untuk mendapatkan nama holder
					string holderName = "Line from " + path[i] + " to " + path[j];
					//tampilkan di log
					debugText.text += "\nTandai path "+path[i] +" > "+path[j];
					//untuk setiap lineholder
					for(int k = 0; k < lineHolder.Length; k++){
						//jika line holder name = holderName
						if(lineHolder[k].name == holderName){
							//aktifkan objek holder
							lineHolder[k].SetActive(true);
							//ubah warna line
							line[k].material = highlightMaterial;
							yield return new WaitForSeconds(1.5f);
						}
					}
				}
			}
		}
	}

	public void markCurrentPath(int startNode, int endNode){
		string holderName = "Line from " + startNode + " to " + endNode;
		for(int k = 0; k < lineHolder.Length; k++){
			if(lineHolder[k].name == holderName){
				line[k].material = traveledMaterial;
			}
		}
	}
	
	//------------- method untuk mencari path string
	public string getPathString(int[] path)
	{
		string pathText = "";
		// Set path text
		for (int j = 0; j < path.Length; j++)
		{
			if (j == 0)
				pathText = "Path : Start ";
			else if (j != path.Length-1)
				pathText = pathText + " -> " + path[j];
			else			
				pathText = pathText + " -> " + "End";
		}
		
		return pathText;
	}
	
	// ------------ method untuk mencari total jarak tempuh dari start > end
	public float getTotalDistance(int[] nodesTraveled)
	{
		float total = 0;
		for(int i = 0; i < nodesTraveled.Length; i++)
		{
			if(i != nodesTraveled.Length - 1)
			{
				total += getTraversalCost(nodesTraveled[i],nodesTraveled[i+1]);
			}
		}
		return total;
	}
	
	//------------- method untuk mendapatkan traversal cost antar node
	public float getTraversalCost(int start, int neighbor)
	{
		//jika terkoneksi
		if (connectionExists [start, neighbor])
			//return nilai distance
			return distances [start, neighbor];
		//atau yg terkoneksi adalah node kebalikannya
		else if (connectionExists[neighbor, start])
			//return juga nilai distance-nya
			return distances[neighbor, start];
		//jika bukan keduanya, berarti nilainya tidak didefinsisikan
		else return Mathf.Infinity; 
		
	}
	
	//----------- method untuk menjalankan algoritma dijkstra
	public int[] runDijkstra()
	{
		//define dijkstra class
		dijkstra = gameObject.GetComponent<Dijkstra> ();
		//buat variable dijksttrapath dengan memanggil method dijkstra getminimumPath
		int[] dijkstraPaths = dijkstra.GetMinimumPath(a, b);
		//jika hasilnya sudah didapat, tampilkan di log
		debugText.text += "\n<color=#800000>Final Path</color>";
		//untuk setiap path
		for (int i = 0; i < dijkstraPaths.Length; i++) {
			//tampilkan di log
			debugText.text +="\n<color=blue>> "+dijkstraPaths[i]+"</color>";
		}
		// return nilai2 path
		return dijkstraPaths;
	}

	//------------method untuk menampilkan nearby atau neighbor
	public IEnumerable<int> GetNearbyNodes(int startingNode)
	{
		//buat list int untuk menyimpan nilai node
		List<int> nearbyNodes = new List<int>();
		//untuk setiap node
		for (int j = 0; j < NumNodes; j++)
		{
			//cek jika node dengan starting node atau sebaliknya exist
			if(connectionExists[startingNode,j] || connectionExists[j,startingNode])
			{
				// jika ya, tambahkan ke list
				nearbyNodes.Add(j);
			}
		}
		//return list 
		return nearbyNodes;
	}
}