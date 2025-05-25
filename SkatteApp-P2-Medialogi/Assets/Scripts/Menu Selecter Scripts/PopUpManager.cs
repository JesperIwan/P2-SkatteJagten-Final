using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpManager : MonoBehaviour

{
    public Transform startpoint;
    public Transform endpoint;
    public Button PopUp;
    public GameObject MenuPopUp;
    public Image RevertImage;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button btn = PopUp.GetComponent<Button>();
        btn.onClick.AddListener(Click);

        StartCoroutine(PulsateRevertImage());
        
    }
    

    void Click()
    {
        AudioManager.Instance.PlaySFX("Click");
        if (MenuPopUp.transform.position == startpoint.transform.position)
        {
            transform.position = endpoint.transform.position;
            RevertImage.rectTransform.Rotate(0,0,180);

        }

        else
        {
            transform.position = startpoint.transform.position;
            RevertImage.rectTransform.Rotate(0,0,180);
            //AudioManager.Instance.PlaySFX("Click");
        }
    }
    IEnumerator PulsateRevertImage()
    {
        while (true)
        {
           for (float t = 0; t < 1; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(1f, 1.2f, t);
                RevertImage.rectTransform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            // Scale down
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                float scale = Mathf.Lerp(1.2f, 1f, t);
                RevertImage.rectTransform.localScale = new Vector3(scale, scale, 1f);
                yield return null;
            }

            // Wait for 2 seconds before repeating
            yield return new WaitForSeconds(2f);
        }
    }

}

