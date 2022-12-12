using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;

    public GameObject itemShop;
	public GameObject weaponShop;
	public GameObject startZone;

    public GameObject mapClass;
    public GameObject mapCafeteria;
    public GameObject mapPc;

	public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;

    public Transform[] enemyZones;
    public GameObject[] enemies;
    public List<int> enemyList;

    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    
    public TextMeshProUGUI maxScoreTxt;
    public TextMeshProUGUI scoreTxt;
    public TextMeshProUGUI stageTxt;
    public TextMeshProUGUI playTimeTxt;
    public TextMeshProUGUI playerHealthTxt;
    public TextMeshProUGUI playerAmmoTxt;
    public TextMeshProUGUI playerCoinTxt;

    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;

    public TextMeshProUGUI enemyATxt;
    public TextMeshProUGUI enemyBTxt;
    public TextMeshProUGUI enemyCTxt;

    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public TextMeshProUGUI curScoreText;
    public TextMeshProUGUI bestText;

    public AudioSource titleBGM;
    public AudioSource stageBGM;
    public AudioSource storeBGM;
    public AudioSource bossBGM;
    public AudioSource dieBGM;

    void Awake()
    {
        enemyList = new List<int>();
        
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        titleBGM.Play();

        if(PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart() 
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);

        mapClass.SetActive(false);
        mapCafeteria.SetActive(false);
        mapPc.SetActive(false);

        titleBGM.Stop();
        stageBGM.Stop();
        storeBGM.Play();
        bossBGM.Stop();
        dieBGM.Stop();
    }

    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore){
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }

        mapClass.SetActive(false);
        mapCafeteria.SetActive(false);
        mapPc.SetActive(false);

        titleBGM.Stop();
        stageBGM.Stop();
        storeBGM.Stop();
        bossBGM.Stop();
        dieBGM.Play();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }


    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        stage++;

        if (stage == 1){
            mapClass.SetActive(true);
            mapCafeteria.SetActive(false);
            mapPc.SetActive(false);
        }
        else if (stage == 3){
            mapClass.SetActive(false);
            mapCafeteria.SetActive(true);
            mapPc.SetActive(false);
        }
        else if (stage == 5){
            mapClass.SetActive(false);
            mapCafeteria.SetActive(false);
            mapPc.SetActive(true);
        }


        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);

        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        //이 부분이 에러나서 주석처리해둡니다. 플레이어 위치 초기화
        // => 책상 위치하고 겹쳐져서 그런것으로 보입니다. 책상 위치를 변경했습니다.
        //player.transform.position = new Vector3(0, 1.24f, -5);
        player.transform.position = Vector3.up * 0.8f;

		itemShop.SetActive(true);
		weaponShop.SetActive(true);
		startZone.SetActive(true);

		foreach (Transform zone in enemyZones)
			zone.gameObject.SetActive(false);

		isBattle = false;
        stage++;
        
        titleBGM.Stop();
        stageBGM.Stop();
        storeBGM.Play();
        bossBGM.Stop();
        dieBGM.Stop();
    }

    IEnumerator InBattle()
    {
		//스테이지가 5단위일때 보스 소환
		if (stage % 7 == 0)
		{
            enemyCntD++;
			GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
			Enemy enemy = instantEnemy.GetComponent<Enemy>();
			enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();

            titleBGM.Stop();
            stageBGM.Stop();
            storeBGM.Stop();
            bossBGM.Play();
            dieBGM.Stop();
		}
		else
		{
            titleBGM.Stop();
            stageBGM.Play();
            storeBGM.Stop();
            bossBGM.Stop();
            dieBGM.Stop();
            
			for (int index = 0; index < stage; index++)
			{

				int ran = UnityEngine.Random.Range(0, 3);
				enemyList.Add(ran);

				switch (ran)
				{
					case 0:
						enemyCntA++;
						break;
					case 1:
						enemyCntB++;
						break;
					case 2:
						enemyCntC++;
						break;
				}
			}

			while (enemyList.Count > 0)
			{
				int ranZone = UnityEngine.Random.Range(0, 4);
				GameObject instantEnemy = Instantiate(enemies[enemyList[0]], enemyZones[ranZone].position, enemyZones[ranZone].rotation);
				Enemy enemy = instantEnemy.GetComponent<Enemy>();
				enemy.target = player.transform;
                enemy.manager = this;
				enemyList.RemoveAt(0);
				yield return new WaitForSeconds(4f);
			}
		}
        //yield return new WaitForSeconds(5);
        //StageEnd();

        while (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0) //아직 못잡은 몬스터 남아있으면 while문
        {
            yield return null;
        }

        //몬스터 다 잡으면 스테이지 종료
        yield return new WaitForSeconds(4f);

        boss = null;
        StageEnd();
    }

    void Update()
    {
        if(isBattle)
            playTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score);
        if (stage == 0){
            stageTxt.text = "Front of Buiding 310";
        }else if (stage == 1){
            stageTxt.text = "Building 310 " + stage + "st floor";
        }else if (stage == 2){
            stageTxt.text = "Building 310 " + stage + "nd floor";
        }else if (stage == 3){
            stageTxt.text = "Building 310 " + stage + "rd floor";
        }else{
            stageTxt.text = "Building 310 " + stage + "th floor";
        }

        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        playerHealthTxt.text = player.health + " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if(player.equipWeapon == null)
            playerAmmoTxt.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoTxt.text = "- / " + player.ammo;
        else 
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

    
        weapon1Img.color = new Color(1,1,1,player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1,1,1,player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1,1,1,player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1,1,1,player.hasGrenade > 0 ? 1 : 0);

        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();


        if(boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
			bossHealthBar.localScale = new Vector3((float)boss.curHealth / (float)boss.maxHealth, 1, 1);
		}
        else
        {
			bossHealthGroup.anchoredPosition = Vector3.up * 200;
		}
            
    }
}
