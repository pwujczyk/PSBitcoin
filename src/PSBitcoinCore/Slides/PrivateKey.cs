﻿using AutoMapper;
using BitcoinSimpleClientObjects;
using NBitcoin;
using PSBitcoinClientCore;
using PSBitcoinDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PSBitcoinCore
{
    public partial class Core
    {
        IMapper Mapper = new Mapper(AutoMapperConfiguration.Configuration);

        BTCEntities entities;
        BTCEntities Entities
        {
            get
            {
                if (entities==null)
                {
                    var conn=ConnectionStringHelper.ConnectionString.GetSqlEntityFrameworkConnectionString(@".\sql2014", "BTCTest", "BitcoinDB");
                    entities = new BTCEntities(conn);
                }
                return entities;
            }
        }


        public string GenerateWIF(BitcoinSimpleClientObjects.Network network)
        {
            Key key = new Key();
            string wif = key.GetWif(Mapper.Map<BitcoinSimpleClientObjects.Network, NBitcoin.Network>(network)).ToString();
            return wif;
        }

        public PrivateKey GeneratePrivateKey(BitcoinSimpleClientObjects.Network network)
        {        
            Key key = new Key();
            PrivateKey result = new PrivateKey();
            result.WIF = key.GetWif(Mapper.Map<BitcoinSimpleClientObjects.Network, NBitcoin.Network>(network)).ToString();
            result.Decimal = PrivateKeyTools.GetPrivateDecimalKey(key);
            result.Binary = PrivateKeyTools.GetPrivateBinaryKey(key);
            return result;
        }

        private static BitcoinDataSet GenerateNewDataSet(NBitcoin.Network network, string wif)
        {
            BitcoinSecret privateKey = new BitcoinSecret(wif);
            BitcoinDataSet btds = new BitcoinDataSet();
            btds.WIF = privateKey.ToWif().ToString();
            btds.PublicKey = privateKey.PubKey.ToString();
            btds.Hash = privateKey.PubKey.Hash.ToString();
            btds.Address = privateKey.PubKey.GetAddress(network).ToString();
            btds.ScriptPubKey = privateKey.PubKey.GetAddress(network).ScriptPubKey.ToString();
            return btds;
        }


        public void  SaveWifToDB(string wif, string name)
        {
            WaletImportFile wifdb = new WaletImportFile();
            wifdb.Name = name;
            wifdb.Wif = wif;
            Entities.WaletImportFile.Add(wifdb);
            Entities.SaveChanges();
        }
    }
}
