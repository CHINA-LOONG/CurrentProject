-- ��̬���ݣ������ݱ�������
-- ����
MonsterData = {
	id,
	name,
	prefab,
	
	hp,
	patk,
	matk,
	pdef,
	mdef,
	speed,
}

-- �Ծ�
BattleData = {
	id,
	-- С�֡�boss��ϡ�й�
	type,
	monsters = {
		{
			id,
			amount,
		}
	},
	processes = {},
}

-- ����
ProcessData = {}