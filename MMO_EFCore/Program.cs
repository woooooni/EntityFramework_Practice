﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace MMO_EFCore
{
    class Program
    {
        
        static void Main(string[] args)
        {
            DbCommands.InitializeDB(forceReset: false);

            //CRUD (Create, Read, Update, Delete)

            Console.WriteLine("명령어를 입력하세요.");
            Console.WriteLine("[0] ForceReset");
            Console.WriteLine("[1] TestDelete");
            

            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine();
                switch (command)
                {
                    case "0":
                        DbCommands.InitializeDB(forceReset: true);
                        break;
                    case "1":
                        DbCommands.TestDelete();
                        break;
                    case "2":
                        break;
                    case "3":
                        break;
                }
            }
        }
    }
}
