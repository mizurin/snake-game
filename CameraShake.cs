using System.Collections;
using UnityEngine;
using DG.Tweening;
public class CameraShake : MonoBehaviour
{
    int up = 0;
    public Rigidbody rb;
    public UnityEngine.UI.Text scoreLabel;
    Vector3 latestPos;
    Vector3 speed;
    public UnityEngine.UI.Text speedLabel;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        latestPos = rb.transform.position;

    }
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    private IEnumerator DoShake(float duration, float magnitude)
    {
        this.transform.DOShakePosition(duration: 1.0f, strength: 0.5f, vibrato: 10, randomness: 10, snapping: false, fadeOut: true);
        yield return new WaitForSeconds(1.0f);
        /*
        var pos = transform.localPosition;

        var elapsed = 0f;

        while (elapsed < duration)
        {
            var x = pos.x + Random.Range(-1f, 1f) * magnitude;
            var z = pos.z + Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(x, pos.y, z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = pos;
        */
    }

   
    public void Upstage(bool tmp)
    {
        if (tmp == true)
        {
            up = 1;//up
        }
        else
        {
            up = 2;//down
        }

    }
    public void OnBrake()
    {
        // コルーチンをIEnumeratorの位置から開始（再開）
        
        up = 3;
    }

    public void OffBrake()
    {
        // コルーチンを停止
        
    }
    
    void FixedUpdate()
    {
        /*
        if (up == 1)
        {
            //rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            rb.AddForce(Vector3.up * 70f, ForceMode.Force);

            up = 0;
        }
        else if (up == 2)
        {
            //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            rb.AddForce(Vector3.down * 150f, ForceMode.Force);

            up = 0;
        }
        else if (up == 3)
        {
            if (rb.transform.position.y - latestPos.y<0)
            {
                rb.AddForce(Vector3.up * -(rb.transform.position.y - latestPos.y)*30f, ForceMode.Force);
            }
            else
            {
                Debug.Log("aa");
                //rb.AddForce(Vector3.up * 50f, ForceMode.Force);
                up = 0;
            }

        }
        */
        /*
        speed= ((this.transform.position - latestPos) / Time.deltaTime);
        latestPos = this.transform.position;

        scoreLabel.text = speed.y.ToString();
        */
    }
}