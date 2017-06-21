using UnityEngine;
using System.Collections;

public class instance_sample : MonoBehaviour {
	//�A�j���[�V��������N���X�@
	private Script_SpriteStudio_Root ScriptRoot;

	int count = 0;		//���Ԃ̃J�E���g
	int type = 0;		//�A�j���[�V�����̎��
	int IDParts;		//�C���X�^���X�p�[�c��ID
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//spritestudio���[�g�N���X�̎擾.
		//spriteStudioRoot���g�p���ăA�j���[�V�����̐�����s���܂�.
		if(null == ScriptRoot)
		{
			//������.
			ScriptRoot = Library_SpriteStudio.Utility.Parts.RootGetChild(gameObject);
			//�A�j���[�V�����Đ�
			ScriptRoot.AnimationPlay(0);	
			//���O����C���X�^���X�p�[�c��ID���擾����
			IDParts = ScriptRoot.IDGetParts("face_base");
		}
		
		count++;
		if ( ( count % 180 ) == 0 )
		{
			string NameAnimation = "";

			//�C���X�^���X�p�[�c���Q�Ƃ��Ă���A�j���[�V������ύX���܂�
			//�Q�ƃA�j���[�V����(ssae)���Ń��[�V������؂�ւ��܂��B
			switch( type )
			{
			case 0:
				NameAnimation = "face2";
				type++;
				break;
			case 1:
				NameAnimation = "face3";
				type++;
				break;
			case 2:
				NameAnimation = "face1";
				type = 0;
				break;
			default:
				break;
			}
			//�Q�ƃA�j���̐؂�ւ��̓A�j���[�V�������~���čs���Ă��������B
			ScriptRoot.AnimationPause(true);
			ScriptRoot.InstanceChange(IDParts,null,NameAnimation);
			ScriptRoot.AnimationPause(false);	
		}
	
	}

}
