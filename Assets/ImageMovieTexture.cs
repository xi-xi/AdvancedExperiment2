using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImageMovieTexture : MonoBehaviour {
	public string ImagesDirName;
	public string ImagePattern;

	private List<string> _image_paths = new List<string> ();
	private int _currentIndex;

	// Use this for initialization
	void Start () {
		this._currentIndex = 0;
		var dirpath = System.IO.Path.Combine (Application.streamingAssetsPath, this.ImagesDirName);
		string[] image_paths = System.IO.Directory.GetFiles (dirpath, this.ImagePattern);
		foreach (var path in image_paths) {
			this._image_paths.Add ( this.PathToWwwUrl(path));
		}
		this.GetComponent<Renderer> ().material.mainTexture = this.LoadTexture (this._image_paths [0]);
	}

	private Texture2D LoadTexture(string path){
		return (new WWW (path)).textureNonReadable;
	}

	private string PathToWwwUrl(string path){
		if (path.Contains ("://")) {
			return path;
		} else {
			return "file://" + path;
		}
	}

	
	// Update is called once per frame
	void Update () {
		this.GetComponent<Renderer> ().material.mainTexture = this.LoadTexture (this._image_paths [this._currentIndex]);
		this._currentIndex++;
		if (this._currentIndex >= this._image_paths.Count) {
			this._currentIndex = 0;
		}
	}
}
