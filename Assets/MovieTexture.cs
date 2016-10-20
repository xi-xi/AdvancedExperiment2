using UnityEngine;
using System.Collections;

public class MovieTexture : MonoBehaviour {

	public UnityEngine.MovieTexture Movie;

	// Use this for initialization
	void Start () {
		this.StartCoroutine (this.moviePlay ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private IEnumerator moviePlay(){
		while (!this.Movie.isReadyToPlay) {
			yield return null;
		}
		var renderer = this.GetComponent<MeshRenderer> ();
		renderer.material.mainTexture = this.Movie;
		this.Movie.loop = true;
		this.Movie.Play ();

		var audioSource = this.GetComponent<AudioSource> ();
		audioSource.clip = this.Movie.audioClip;
		audioSource.loop = true;
		audioSource.Play ();
	}
}
