DROP TABLE IF EXISTS `orders`;
CREATE TABLE `orders` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `myOrder` varchar(255) COLLATE utf8_bin NOT NULL,
  `pfOrder` varchar(255) COLLATE utf8_bin DEFAULT NULL,
  `suuid` varchar(255) COLLATE utf8_bin DEFAULT NULL,
  `game` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `platform` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `serverId` int(11) NOT NULL DEFAULT 0,
  `channel` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `playerId` int(11) NOT NULL DEFAULT 0,
  `puid` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `device` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `goodsId` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `goodsCount` int(11) NOT NULL DEFAULT 0,
  `orderMoney` int(11) NOT NULL DEFAULT 0,
  `payMoney` int(11) NOT NULL DEFAULT 0,
  `currency` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `addGold` int(11) NOT NULL DEFAULT 0,
  `giftGold` int(11) NOT NULL DEFAULT 0,
  `payPf` varchar(255) COLLATE utf8_bin DEFAULT NULL,
  `userData` varchar(4096) COLLATE utf8_bin DEFAULT '',
  `status` int(11) NOT NULL DEFAULT 0,
  `date` varchar(64) COLLATE utf8_bin NOT NULL DEFAULT '0000-00-00',
  `createTime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `updateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY (`myOrder`),
  UNIQUE KEY (`pforder`),
  KEY `myOrder_index` (`myOrder`) USING BTREE,
  KEY `payPf_index` (`payPf`) USING BTREE,
  KEY `pfOrder_index` (`pfOrder`) USING BTREE,
  KEY `suuid_index` (`suuid`) USING BTREE,
  KEY `game_index` (`game`) USING BTREE,
  KEY `game_platform_index` (`game`, `platform`) USING BTREE,
  KEY `game_channel_index` (`game`, `channel`) USING BTREE,
  KEY `playerId_index` (`playerId`) USING BTREE,
  KEY `puid_index` (`puid`) USING BTREE,
  KEY `game_puid_index` (`game`, `puid`) USING BTREE,
  KEY `currency_index` (`currency`) USING BTREE,
  KEY `goodsId_index` (`currency`) USING BTREE,
  KEY `status_index` (`status`) USING BTREE,
  KEY `date_index` (`date`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;


DROP TABLE IF EXISTS `callback`;
CREATE TABLE `callback` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `myOrder` varchar(255) COLLATE utf8_bin NOT NULL,
  `pfOrder` varchar(255) COLLATE utf8_bin NOT NULL DEFAULT '',
  `payMoney` int(11) NOT NULL DEFAULT 0,
  `payPf` varchar(255) COLLATE utf8_bin DEFAULT '',
  `userData` varchar(4096) COLLATE utf8_bin DEFAULT '',
  `status` int(11) NOT NULL DEFAULT 0,
  `date` varchar(64) COLLATE utf8_bin NOT NULL DEFAULT '0000-00-00',
  `createTime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `updateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY (`myOrder`),
  UNIQUE KEY (`pforder`),
  KEY `myOrder_index` (`myOrder`) USING BTREE,
  KEY `pfOrder_index` (`pfOrder`) USING BTREE,
  KEY `status_index` (`status`) USING BTREE,
  KEY `date_index` (`date`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;


DROP TABLE IF EXISTS `request`;
CREATE TABLE `request` (
  `id` int(11) NOT NULL AUTO_INCREMENT,    
  `game` varchar(255) COLLATE utf8_bin NOT NULL,
  `channel` varchar(255) COLLATE utf8_bin NOT NULL,
  `pfOrder` varchar(255) COLLATE utf8_bin NOT NULL,
  `request` varchar(8192) COLLATE utf8_bin NOT NULL,
  `state` int(11) NOT NULL DEFAULT 0,
  `date` varchar(64) COLLATE utf8_bin NOT NULL DEFAULT '0000-00-00',
  `createTime` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00',
  `updateTime` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`),
  UNIQUE KEY (`pfOrder`),
  KEY `game_index` (`game`) USING BTREE,
  KEY `game_channel_index` (`game`, `channel`) USING BTREE,
  KEY `date_index` (`date`) USING BTREE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;


DROP TABLE IF EXISTS `mycard`;
CREATE TABLE IF NOT EXISTS `mycard` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `game` varchar(255) NOT NULL,
  `paytype` varchar(255) NOT NULL,
  `puid` varchar(255) NOT NULL,
  `myOrder` varchar(255) NOT NULL,
  `PaymentAmount` int(11) NOT NULL,
  `authcode` varchar(255) NOT NULL,
  `state` tinyint(2) NOT NULL,
  `create_time` varchar(64) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `myOrder` (`myOrder`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 COMMENT='mycard_bingling';
