using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RefillArea : MonoBehaviour
{
    [SerializeField]
    GameObject espada;
    [SerializeField]
    GameObject lanzaIzquierda;
    [SerializeField]
    GameObject lanzaDerecha;
    // Start is called before the first frame update
    void Start()
    {
        foreach (XRSocketInteractor socket in GameObject.FindObjectsOfType<XRSocketInteractor>())
        {
            
            if (!socket.hasSelection)
            {
                switch (socket.interactionLayers.value)
                {
                    case 8: // Espada
                        Instantiate(espada).transform.position = socket.gameObject.transform.position;
                        break;
                    case 4: // Lanza Derecha
                        Instantiate(lanzaDerecha).transform.position = socket.gameObject.transform.position;
                        break;
                    case 2: // Lanza Izquierda
                        Instantiate(lanzaIzquierda).transform.position = socket.gameObject.transform.position;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
            foreach(XRSocketInteractor socket in GameObject.FindObjectsOfType<XRSocketInteractor>())
			{
                if (socket.hasSelection)
                    Debug.Log("Tengo un objeto:" + socket.gameObject.name);
				if (!socket.hasSelection)
				{
					switch (socket.interactionLayers.value)
					{
                        case 8: // Espada
                            Instantiate(espada).transform.position = socket.gameObject.transform.position;
                            break;
                        case 4: // Lanza Derecha
                            Instantiate(lanzaDerecha).transform.position = socket.gameObject.transform.position;
                            break;
                        case 2: // Lanza Izquierda
                            Instantiate(lanzaIzquierda).transform.position = socket.gameObject.transform.position;
                            break;
                        default:
                        break;
					}
				}
			}
		}
	}
}
