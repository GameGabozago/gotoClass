using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Specialized;
using System.Security.Cryptography;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;

    public GameObject itemShop;
	public GameObject weaponShop;
	public GameObject startZone;

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


    public AudioSource titleBGM;
    public AudioSource stageBGM;
    public AudioSource storeBGM;

    void Awake()
    {
        enemyList = new List<int>();
        
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));

        titleBGM.Play();
    }

    public void GameStart() 
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        player.gameObject.SetActive(true);

        titleBGM.Stop();
        storeBGM.Play();
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);

        foreach (Transform zone in enemyZones)
            zone.gameObject.SetActive(true);



        isBattle = true;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        //이 부분이 에러나서 주석처리해둡니다. 플레이어 위치 초기화
        player.transform.position = new Vector3(0, 1.24f, -5);

		itemShop.SetActive(true);
		weaponShop.SetActive(true);
		startZone.SetActive(true);

		foreach (Transform zone in enemyZones)
			zone.gameObject.SetActive(false);

		isBattle = false;
        stage++;
    }

    IEnumerator InBattle()
    {
		//스테이지가 5단위일때 보스 소환
		if (stage % 5 == 0)
		{
            enemyCntD++;
			GameObject instantEnemy = Instantiate(enemies[3], enemyZones[0].position, enemyZones[0].rotation);
			Enemy enemy = instantEnemy.GetComponent<Enemy>();
			enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
		}
		else
		{
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
        stageTxt.text = "310관 " + stage + "층";

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
