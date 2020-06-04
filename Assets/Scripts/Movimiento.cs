using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class Movimiento : MonoBehaviour
{
    public PlayerController controller;
    public float velocidadMovimiento = 40f;
    float movimientoVertical = 0f;
    float movimientoHorizontal = 0f;
    bool agachar = false;
    bool salto = false;
    public Animator animator;
    [SerializeField] public int cerezas = 0;
    [SerializeField] public TextMeshProUGUI cerezasText;
    [SerializeField] public int gems = 0;
    [SerializeField] public TextMeshProUGUI gemsText;
    [SerializeField] private AudioSource cerezaSonido;
    [SerializeField] private AudioSource gemasSonido;
    [SerializeField] public TextMeshProUGUI healthText;
    [SerializeField] int health = 100;
    [SerializeField] private AudioSource death;
    [SerializeField] private AudioSource heridoSonido;
    private int max_coins = 0;
    private int max_gems = 0;

    public GameObject coinsReference;
    public GameObject gemsReference;
    public bool herido = true;




    // Start is called before the first frame update
    private void Start()
    {
        this.max_coins = coinsReference.transform.childCount + 1;
        this.max_gems = gemsReference.transform.childCount + 1;
        this.healthText.text = health.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * velocidadMovimiento;
        movimientoVertical   = Input.GetAxisRaw("Vertical") * velocidadMovimiento;

        animator.SetFloat("velocidad", Mathf.Abs(movimientoHorizontal));
        
        if (Input.GetButtonDown("Jump"))
        {
            salto = true;
            animator.SetBool("estaSaltando", true);
        }
        if (Input.GetButtonDown("Crouch"))
        {
            agachar = true;
        } else if (Input.GetButtonUp("Crouch"))
        {
            agachar = false;
        }
    }

    public void enElSuelo()
    {
        animator.SetBool("estaSaltando", false);
    }

    public void alAgacharse(bool estaAgachado)
    {
        animator.SetBool("estaAgachado", estaAgachado);
    }

    private void FixedUpdate()
    {
        if (!herido)
        {
            controller.Move(movimientoHorizontal * Time.fixedDeltaTime , movimientoVertical * Time.fixedDeltaTime, agachar, salto);
        }  else
        {
            StartCoroutine(waiter());
        }
        //Delta time para no tener lag
        salto = false;
        herido = false;        
    }

    IEnumerator waiter ()
    {
        //metodo para esperar cuando nos han herido
        yield return new WaitForSeconds(1);
        animator.SetBool("herido", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemigo")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            //Si la Y del personaje es mas grande que la del enemigo y ha colisinado con el significa que lo ha matado
            if (Math.Round(collision.gameObject.transform.position.y) < Math.Round(transform.position.y))
            {
                enemy.JumpedOn();
                salud("sumar", 5);

                salto = true;
                animator.SetBool("estaSaltando", true);
            } else
            {
                // Sino, es que ha sido herido
                heridoSonido.Play();
                herido = true;
                salud("restar", 20);
                animator.SetBool("herido", true);
                controller.Hurt(collision.gameObject.transform.position.x, transform.position.x);
            }
        }
    }
    /*
     * Funcion para sumar / restar salud y matar al personaje si esta a menos de 0 de vida
     */
    private void salud(String accion, int valor)
    {
        if (accion =="sumar")
        {
            health += valor;

        } else
        {
            health -= valor;
        }
        healthText.text = health.ToString();
        if (health <= 0)
        {
            death.Play();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si colisinamos con un coin / gem, reproducimos el sonido, borramos el gameObject y hacemos el recuento 
        if (collision.tag == "Coin")
        {
            cerezaSonido.Play();
            Destroy(collision.gameObject);
            cerezas = this.max_coins - (coinsReference.transform.childCount);
            cerezasText.text = cerezas.ToString();
        }
        if (collision.tag == "Gem")
        {
            gemasSonido.Play();
            Destroy(collision.gameObject);
            gems = this.max_gems - (gemsReference.transform.childCount);
            gemsText.text = gems.ToString();
        }
        //si colisionamos en una escalera cambiamos las variables y tiggereamos la animacion
        if (collision.tag == "Ladder")
        {
            controller.onLadder = true;
            animator.SetBool("trepando", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Si salimos de una colision y era la escalera, ponemos a false la animacion y el booleano
        if (collision.tag == "Ladder")
        {
            controller.onLadder = false;
            animator.SetBool("trepando", false);
        }
    }
}
