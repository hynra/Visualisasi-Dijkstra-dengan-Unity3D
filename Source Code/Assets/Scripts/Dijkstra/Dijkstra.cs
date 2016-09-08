//==================================================================
// Node Configurator class : digunakan untuk mencari jarak terpendek
//==================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class Dijkstra : MonoBehaviour{
	//define node configurator
	private NodeConfigurator nodeConfigurator;
	//define text log
	private Text debugText;
	//variable untuk menyimpan jumlah nodes
	private int TotalNodeCount;
	
	void Start(){
		//init nodeconfigurator
		nodeConfigurator = gameObject.GetComponent<NodeConfigurator> ();
		//init log text
		debugText = nodeConfigurator.debugText;
	}
	

	
	//buat struct dengan nama Results, untuk menentukan tiap path dan jaraknya
	public struct Results
	{
		// variable untuk path
		public int[] MinimumPath;
		// variable untuk distance
		public float[] MinimumDistance;
		public Results(int[] minimumPath, float[] minimumDistance)
		{
			
			MinimumDistance = minimumDistance;
			MinimumPath = minimumPath;
			
		}

	}
	//-------------method struct untuk menentukan hasil djikstra
	public Results Perform(int start)
	{
		//get starting travelcost
		float[] d = GetStartingTraversalCost(start);
		//set bestpath, set semua menjadi start, misal 0
		int[] p = GetStartingBestPath(start);
		// define basic heap
		BasicHeap Q = new BasicHeap();
		//tampilkan di log
		debugText.text += "\n<color=#800000>Starting Traversal Cost from node 0</color>";
		//jika i != total node
		for (int i = 0; i != TotalNodeCount; i++) {
			//masukkan nilai starting path & starting travesalcost kedalam heap
			Q.Push (i, d [i]); 
			//tampilkan di log
			if(d[i] != Mathf.Infinity){
				// neighbor dr starting node
				debugText.text += "\n<color=#0000ff>node "+i+" = "+d[i]+"</color>";
			}
			else{
				//node yang belum terjamah
				debugText.text += "\n<color=#0000ff>node "+i+" = "+d[i]+"</color>";
			}
		}
		//tampilkan di debug
		debugText.text += "\n<color=#800000>Start Main Loop</color>";
		// jika Q (node yang harus di cek) lebih dari 0
		while (Q.Count > 0)
		{
			// buat variable v (visited / current node) dari nilai heap paling awal
			int v = Q.Pop();
			//tampilkan di log
			debugText.text += "\n<color=blue>[---CEK NODE "+v+"---] \n> Remaining Q = "+Q.Count+"] : </color> ";
			// tampilkan di log node dan traversalcost ned yang belum terjamah / yg harus di cek
			for(int i = 0; i < Q.Count; i++){
				//tampilkan di log
				debugText.text += "\nnode "+Q.getIndex(i)+" : ";
				debugText.text +="cost : "+Q.getWight(i);
			}
			//tampilkan di log
			debugText.text += "\n<color=blue>> Cek Nearby dari node "+v+ " : </color>";
			//untuk setiap neighbor dari node yang sedang di cek
			foreach (int w in nodeConfigurator.GetNearbyNodes(v))
			{
				//tampilkan di log
				debugText.text += "\nnode "+v+" > node "+w;
				//tentukan traversalcost dari current node ke neighbor
				float cost = nodeConfigurator.getTraversalCost(v, w);
				//tandai node yang telah dilalui
				nodeConfigurator.markCurrentPath(v, w);
				//tampilkan di log :
				debugText.text += ", cost = "+cost;
				//jika traversal cost V + cost ke neighbor yg sedang di cek 'w' < dari traversal cost sebelumnya
				if (d[v] + cost < d[w])
				{
					//tampilkan di log
					debugText.text += "\nTravecost dari "+v+" > "+w+ " = "+(d[v]+cost)+"\n<color=#660066>Lebih kecil dari cost yg sekarang = "+d[w]+"</color>";
					//masukan nilai baru hasil pertambahan current ke neighbor yang di cek ke dalam travelcost yng sedang di cek
					d[w] = d[v] + cost;
					//tampilkan di log
					debugText.text +="\n> Set cost : dr node sebelumnya > "+v+" > "+w+" = "+d[w];
					debugText.text += "\n> Set best path menuju "+w+" dari = "+p[w]+" menjadi = "+v;
					// set best path menjadi node yang di cek
					p[w] = v;
					//tampilkan di log
					debugText.text += ">\nPush index = "+w+", weight = "+d[w]+" ke Q untuk dicek selanjutnya";
					//push node dan weightnya ke heap
					Q.Push(w, d[w]);
					//tampilkan di log :
					debugText.text += "\njadi Q = "+Q.Count;
					//jika perbandingan harganya sama
				}else if(d[v] + cost == d[w]){
					//tampilkan di log :
					debugText.text += "\nTravecost dari "+v+" > "+w+ " = "+(d[v]+cost)+"\n<color=#006600>Harga yang sama dari cost yg sekarang = "+d[w]+"</color>";
					//jika perbandingan harganya lebih besar
				}else if(d[v] + cost > d[w]){
					//tampilkan di log :
					debugText.text += "\nTravecost dari start > "+v+" > "+w+ " = "+(d[v]+cost)+"\n<color=#006600>Harga yang lebih besar dari cost yg sekarang = "+d[w]+"</color>";
				}
			}
		}
		
		//tampilkan di log
		debugText.text += "\n<color=#800000>Node hasil perhitungan</color>";
		// cek masing2 nilai p sekarang
		for (int i = 0; i < p.Length; i++) {
			//tampilkan di log
			debugText.text += "\nStart > node "+i+" melalui node "+p[i];
		}
		//tampilkan di log :
		debugText.text += "\n<color=#800000>Distance dari start ke masing2 node</color>";
		//cek nilai d baru
		for (int i = 0; i < d.Length; i++) {
			//tampilkan di log :
			debugText.text += "\n> Start > "+i+" = "+d[i];
		}
		//return nilia p dan d
		return new Results(p, d);
	}
	//method untuk menentukan jarak terpendek
	public int[] GetMinimumPath(int start, int finish)
	{ 
		//set total node
		TotalNodeCount = nodeConfigurator.NumNodes;
		//lakukan pencarian dengan memanggil method 'pERFORM'
		Results results = Perform(start);
		//Tampilkan di log :
		debugText.text += "\n<color=#800000>Step menuju start point</color>";
		//masukan hasil pencarian terpendek kedalam variable X, kemudian sortir
		int[] x =  GetMinimumPath(start, finish, results.MinimumPath);
		//tampilkan di log :
		debugText.text += "\n<color=#800000>Node optimal Path</color>";
		//periksa setiap nilai dari x
		for (int i = 0; i < x.Length; i++) {
			//tampilkan di log :
			debugText.text += "\n> "+x[i];
		}
		//kembalikan nilai X
		return x;
	}
	private int[] GetMinimumPath(int start, int finish, int[] shortestPath)
	{
		foreach (int i in shortestPath) {
			debugText.text += "\n> sortestpath "+i;
		}
		//buat stack
		Stack<int> path = new Stack<int>();
		//lakukan loop sampai nilai path == nilai awal / node start
		do
		{
			//tampilkan di log :
			debugText.text += "\n<color=blue>> "+finish+"</color>";
			// masukan nilai paling akhir terlebih dahuli
			path.Push(finish);
			//ganti nilai finish dengan path paling akhir // shortestPath[finish] = path paling ahir
			//.. update nilai finish
			finish = shortestPath[finish];
		}
		while (finish != start);
		//tampilkan di log :
		debugText.text += "\n<color=blue>> "+finish+"</color>";
		//push finish karena nilai paling akhir pasti start node
		path.Push (finish);
		//return nilai stack menjadi array
		return path.ToArray();
	} 
	// method untuk menentukan semua path dari 0
	private int[] GetStartingBestPath(int startingNode)
	{
		//get totoal node masukan ke p
		int[] p = new int[TotalNodeCount];
		//cek semua node
		for (int i = 0; i < p.Length; i++)
			// jadikan nilai p starting node
			p[i] = startingNode;
		//return
		return p;
	}

	//method untuk mencari starting traversal cost
	private float[] GetStartingTraversalCost(int start)
	{
		//buat variable subset dari jumalh node
		float[] subset = new float[TotalNodeCount];
		//cek untuk setiap subset
		for (int i = 0; i != subset.Length; i++) {
			//rubah menjadi infinity,
			subset [i] = Mathf.Infinity;
		}
		//kecuali node start
		subset[start] = 0;
		// dan tetangganya
		foreach (int nearby in nodeConfigurator.GetNearbyNodes(start)) {
			//dibuat bernilai dan tidak infinity
			subset [nearby] = nodeConfigurator.getTraversalCost (start, nearby);
		}
		//return nilai subset
		return subset;
	}

}