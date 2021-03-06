﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BoardManager : MonoBehaviour
{

	public GameObject dot;

	private List<Body> bodys = new List<Body> ();
	private bool compute;
	private bool displayQuad;
	private bool circle;
	private bool bruteForce;
	private int size;
	private double framecount = 0;
	private Boundary boundary;
	private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch ();
	private QuadNode quadTree;
	private float dist;
	private List<Quad> quads = new List<Quad> ();

	void Start ()
	{
		boundary = new Boundary (1000);
		dist = 0;
		compute = false;
		circle = false;
		bruteForce = false;
		displayQuad = true;
		size = bodys.Count;
		Debug.Log ("##### &start =#####");
		for(int i = -10;i<10;i++){
			for(int j = -10;j<10;j++){
				GameObject dotGO = Instantiate (dot, new Vector3 (i*2, j*2, 0), Quaternion.identity) as GameObject;
				bodys.Add (new Body (dotGO));
			}
		}

	}

	// Update is called once per frame
	void Update ()
	{
		createBodyIfNeeded ();

		boundary.update (bodys);
		float sized = Mathf.Max ((boundary.max.x - boundary.min.x), (boundary.max.y - boundary.min.y));
		Vector3 center = new Vector3 ((boundary.max.x + boundary.min.x) / 2, (boundary.max.y + boundary.min.y) / 2, (boundary.max.z + boundary.min.z) / 2);
		quadTree = new QuadNode (1, center, sized);

		stopwatch.Start();
		foreach (Body bod in bodys) {
			quadTree.addBody (bod);
		}
		if (compute) {
		
			if(bruteForce)
				bruteFroceUpdate();
			else
				BarnesHut ();
		}
		stopwatch.Stop();
		Debug.Log("plop "+bodys.Count+";"+stopwatch.ElapsedTicks);
		stopwatch.Reset();
	}

	void BarnesHut(){
		foreach (Body body in bodys){
			quadTree.interact(body,0.5f);
			body.update();
		}
	}

	void bruteFroceUpdate ()
	{
		foreach (Body bodyFirst in bodys) {
			foreach (Body bodySecond in bodys) {
				bodyFirst.interac (bodySecond);
			}
			bodyFirst.update ();
		}
	}

	private void createBodyIfNeeded ()
	{
		if (Input.GetKeyDown ("space"))
			compute = !compute;

		if (Input.GetKeyDown ("b"))
			displayQuad = !displayQuad;

		if (Input.GetKeyDown ("n"))
			bruteForce = !bruteForce;


		if (Input.GetButtonDown ("Fire1")) {
			//circle = !circle;
			Vector3 mouse = Input.mousePosition;
			mouse = Camera.main.ScreenToWorldPoint (mouse);
			mouse.z = 0;
			GameObject dotGO = Instantiate (dot, mouse, Quaternion.identity) as GameObject;
			bodys.Add (new Body (dotGO));
			size = bodys.Count;
		}
		framecount++;
		if (circle) {
			GameObject dotGO = Instantiate (dot, new Vector3 (Mathf.Cos (Time.time) * dist, Mathf.Sin (Time.time) * dist, 0), Quaternion.identity) as GameObject;
			bodys.Add (new Body (dotGO));
			dist+=0.05f;
			size = bodys.Count;
		}
	}

	public void OnDrawGizmos ()
	{
		if (displayQuad) {
			quadTree.getAllQuad (quads);
			foreach (Quad quad in quads) {
				Gizmos.color = quad.color;
				Gizmos.DrawWireCube (quad.position, quad.size);
				if(quad.gravityCenter != Vector3.zero)
		 			Gizmos.DrawSphere(quad.gravityCenter,quad.mass);
			}
			quads.Clear ();
		}
	}
}
