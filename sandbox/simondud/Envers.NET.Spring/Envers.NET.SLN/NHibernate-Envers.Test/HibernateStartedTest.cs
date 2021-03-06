﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Proxy;
using NUnit.Framework;
using Spring.Context.Support;
using Spring.Context;
using NHibernate;
using Envers.Net.Model;
using Envers.Net.Repository;
using Spring.Data.Common;
using System.Data;
using NHibernate.Envers.Event;
using Spring.Data.NHibernate;
using NHibernate.Impl;

namespace APDRP_NHibernatePOC_Test
{
    [TestFixture]
    public class HibernateStartedTest
    {
        private IApplicationContext applicationContext;
        private IRepository<Person> personRepository;
        private IRepository<Address> addressRepository;
        private IRepository<ContBancar> contRepository;

        [TestFixtureSetUp]
        public void SetUp()
        {
            applicationContext = new XmlApplicationContext("spring-config.xml");
            personRepository = (IRepository<Person>)applicationContext.GetObject("PersonRepository");
            addressRepository = (IRepository<Address>)applicationContext.GetObject("AddressRepository");
            contRepository = (IRepository<ContBancar>)applicationContext.GetObject("ContBancarRepository");
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            ((XmlApplicationContext)applicationContext).Dispose();
        }

        //[Test]
        public void testDbProvider()
        {
            IDbProvider dbProvider = (IDbProvider)applicationContext.GetObject("DbProvider");
            using(IDbConnection connection = dbProvider.CreateConnection())
            {
                Assert.NotNull(connection);
            }
        }


        //[Test]
        public void TestSessionFactoryStartedOk()
        {
            ISessionFactory sessionFactory = (ISessionFactory)applicationContext.GetObject("NHibernateSessionFactory");
            Assert.IsNotNull(sessionFactory);
        }


        [Test]
        public void TestSessionFactory()
        {
            //AuditEventListener lis = (AuditEventListener)applicationContext.GetObject("enversEventListener");
            Address adr = new Address { number = "16", street = "Diligentei" };
            addressRepository.Add(adr);
            ContBancar cnt = new ContBancar { NumeBanca = "BCR", IBAN = "RO212BCRO423455634", Adresa = adr };
            contRepository.Add(cnt);
            Person persoana = new Person { firstName = "Mihai", lastName = "Popa", cont = cnt};
            personRepository.Add(persoana);
            persoana.lastName = "Popatatu";
            personRepository.Update(persoana);
            IList personRevisionIds;
            //personRevisionIds = personRepository.GetAllRevisionIds(persoana);

            //foreach (object personRevId in personRevisionIds)
            //{
            //    Person auditedPerson = personRepository.GetRevision(typeof(Person), persoana.id, (long)(int)personRevId);
            //    Assert.IsTrue(auditedPerson.cont is SessionImpl);
            //    System.Console.Write(personRevId + ":" + auditedPerson.firstName + " " + auditedPerson.lastName + " -> " + auditedPerson.address.street);
            //}


            Address address = new Address{number="22", street="Valea Calugareasca"};
            addressRepository.Add(address);
            Assert.IsTrue(address.id != 0);
            long id1 = address.id;

            address.number = "29";
            addressRepository.Update(address);
            
            Person pers = new Person{firstName = "Ion", lastName = "Gheorghe", address = address};
            personRepository.Add(pers);
            Assert.IsTrue(pers.id != 0);

            pers.firstName = "Ionel";
            personRepository.Update(pers);
            
            personRevisionIds = personRepository.GetAllRevisionIds( pers );

				foreach ( object personRevId in personRevisionIds )
				{
					Person auditedPerson = personRepository.GetRevision( typeof(Person), pers.id, (long)(int)personRevId );
                    if (auditedPerson.address is INHibernateProxy)
                    {
                        Type tt = ((INHibernateProxy)auditedPerson.address).HibernateLazyInitializer.PersistentClass;
                        object gigi = ((INHibernateProxy)auditedPerson.address).HibernateLazyInitializer.GetImplementation();
                    }
				    Type t = NHibernateUtil.GetClass(auditedPerson.address);

					System.Console.Write( personRevId + ":" + auditedPerson.firstName + " " + auditedPerson.lastName + " -> " + auditedPerson.address.street );
				}


            address.number = "23";
            addressRepository.Update(address);

            address.number = "24";
            addressRepository.Update(address);

            address = new Address { number = "45", street = "Strada Strada" };
            addressRepository.Add(address);

            ContBancar cont = new ContBancar{NumeBanca="BRD", IBAN = "RO212423455634",Adresa = address};

            contRepository.Add(cont);

            pers.cont = cont;
            personRepository.Update(pers);

            address = new Address { number = "55bis", street = "Alta strada" };
            addressRepository.Add(address);

            ContBancar cont2 = new ContBancar { NumeBanca = "BCR", IBAN = "RO33BCRO212423455634", Adresa = address };
            contRepository.Add(cont2);

            pers.cont = cont2;
            personRepository.Update(pers);

            cont.IBAN += "P2";
            contRepository.Update(cont);

            Person pers2 = new Person { firstName = "Mihai", lastName = "Trompetzescu", address = address, cont = cont};
            personRepository.Add(pers2);

            address = addressRepository.GetById(id1);
            Assert.IsTrue(address.number == "24" && address.street == "Valea Calugareasca");

            address = new Address { number = "123", street = "P-asta o stergem"};
            addressRepository.Add(address);

            addressRepository.Remove(address);

        }
    }
}
