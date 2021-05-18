using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public LayerMask whatcanbekicked;
	public float movespeed;
	public float dodgespeed;
	public float chargespeed;
	public float mousesensitivity;
	public float kickrange;
	public Camera viewcam;
	public GameObject Gun;
	public GameObject arrow;
	public GameObject grapplearrow;
	public GameObject Kick;
	public GameObject vaultkick;
	public GameObject dashscreen;
	public GameObject slowmoscreen;
	public Transform arrowspawnpos;
	public Vector3 movehorizontal;
	public Vector3 moveVertical;
	public bool shooting;
	public GameObject enemytodashto;
	public float slowdownfactor;
	public float slowdownlength;
	public int health;
	public float minangleofrotation;
	public float maxangleofrotation;
	public GameObject greencrosshair;
	public GameObject[] hearts;
}
