using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MjpegTexture : MonoBehaviour {
	public string MjpegFileName;
	public float Fps = 30.0f;

	private byte[] _mjpeg_movie_data;
	private int _currentIndex;
	private List<int> _soi_indices = new List<int>();
	private List<int> _eoi_indices = new List<int>();
	private Texture2D _current_texture;
	bool _movie_loaded = false;

	// Use this for initialization
	void Start () {
		GameObject.Find ("Text").GetComponent<Text> ().text = "Start";
		this._currentIndex = 0;
//		GameObject.Find ("Text").GetComponent<Text> ().text = "Cashed";
		this._current_texture = new Texture2D (2, 2);
		this.StartCoroutine (this.LoadMovie());
	}

	private IEnumerator LoadMovie(){
		this._movie_loaded = false;
		#if UNITY_EDITOR
		var filepath = System.IO.Path.Combine (
			Application.streamingAssetsPath,
			this.MjpegFileName
		);
		#elif UNITY_ANDROID
		var filepath = System.IO.Path.Combine(
		"jar:file://" + Application.dataPath + "!/assets" ,
		this.MjpegFileName
		);
		#endif
		GameObject.Find ("Text").GetComponent<Text> ().text = "Loading";
		using(WWW www = new WWW(this.PathToWwwUrl(filepath))){
			yield return www;
			GameObject.Find ("Text").GetComponent<Text> ().text = "Loaded";
			if (!string.IsNullOrEmpty(www.error)) {
				GameObject.Find ("Cube").GetComponent<Renderer> ().material.color = new Color (0.0f, 0.0f, 1.0f);
				GameObject.Find ("Text").GetComponent<Text> ().text = "Error:" + www.error;
			}
			this._mjpeg_movie_data = www.bytes;
			for (int i = 0; i < this._mjpeg_movie_data.Length - 1; ++i) {
				if (this.IsSOI (this._mjpeg_movie_data, i)) {
					this._soi_indices.Add (i);
				} else if (this.IsEOI (this._mjpeg_movie_data, i)) {
					this._eoi_indices.Add (i);
				}
			}
			this._movie_loaded = true;
		}
	}

	private bool IsSOI(byte[] data, int index){
		return (data [index] == ((byte)0xff)) &&
		(data [index + 1] == ((byte)0xd8));
	}

	private bool IsEOI(byte[] data, int index){
		return (data [index] == ((byte)0xff)) &&
			(data [index + 1] == ((byte)0xd9));
	}

	private string PathToWwwUrl(string path){
		if (path.Contains ("://")) {
			return path;
		} else {
			return "file://" + path;
		}
	}

	private void SetMovieFrame(int i){
		if (!this._movie_loaded) {
			return;
		}
		int bufsize = this._eoi_indices [i] + 1 - this._soi_indices [i] + 1;
		byte[] buf = new byte[bufsize];
		System.Buffer.BlockCopy (this._mjpeg_movie_data, this._soi_indices [i], buf, 0, bufsize);
		var flag = this._current_texture.LoadImage (buf);
		if (flag) {
			GameObject.Find ("Cube").GetComponent<Renderer> ().material.color = new Color (0.0f, 1.0f, 0.0f);
		} else {
			GameObject.Find ("Cube").GetComponent<Renderer> ().material.color = new Color (1.0f, 0.0f, 0.0f);
		}
		this.GetComponent<Renderer> ().material.mainTexture = this._current_texture;

	}

	// Update is called once per frame
	void Update () {
		this.SetMovieFrame (this._currentIndex);
		int upframe = Mathf.RoundToInt (Time.deltaTime * this.Fps);
		this._currentIndex += upframe;
		if (this._currentIndex >= this._soi_indices.Count) {
			this._currentIndex = this._currentIndex - this._soi_indices.Count;
		}
	}
}
