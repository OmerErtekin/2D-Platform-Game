using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Train : MonoBehaviour
{
    private PlayerMove playerScript;
    public Transform mineTransform;
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameObject.Find("Player").GetComponent<PlayerMove>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerScript.isTimeToGo == true)
        {
            StartCoroutine(GoNextLevel());
        }

    }

    IEnumerator GoNextLevel()
    {
        playerScript.canMove = false;
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(mineTransform.position.x, transform.position.y), 2.5f * Time.deltaTime);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Level 2");

    }
    
}
