﻿using System.Collections.Generic;
using System.Linq;
using SocialNetwork2.Library.Models;

namespace SocialNetwork2.Library.Implementations
{
    public class User
    {
        public User(string userName)
        {
            //
        }

        public void Post(string message)
        {
            messages.Push(new Message(message));
        }

        public IEnumerable<string> Read()
        {
            return messages.Select(it => it.ToString());
        }

        public void Follow(string otherUser)
        {
            //
        }

        public IEnumerable<string> Wall()
        {
            return Enumerable.Empty<string>();
        }

        //

        private readonly Stack<Message> messages = new Stack<Message>();
    }
}