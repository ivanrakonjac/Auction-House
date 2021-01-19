using AuctionHouse.Models.Database;
using Microsoft.AspNetCore.Identity;

namespace AuctionHouse.Models.Initializers {
    public class UserInitializer {
        public static string[][] users = new string[][] {
            new string[] { "Glennis","Lippini","glippini0","glippini0@indiegogo.com","UbrXJYp6A" },
            new string[] { "Neel","Hulcoop","nhulcoop1","nhulcoop1@google.fr","ZLkppMCrF" },
            new string[] { "Sharlene","Boulsher","sboulsher2","sboulsher2@plala.or.jp","VMpdIshSpQ7" },
            new string[] { "Imogen","Chatten","ichatten3","ichatten3@parallels.com","qtcBh5R6I" },
            new string[] { "Shaun","Fendley","sfendley4","sfendley4@storify.com","BuH5F8Bb1" },
            new string[] { "Tony","Iozefovich","tiozefovich5","tiozefovich5@mit.edu","pT9NqChHOWz" },
            new string[] { "Miriam","Stanners","mstanners6","mstanners6@simplemachines.org","ULnMVTiH0" },
            new string[] { "Lucy","Maryska","lmaryska7","lmaryska7@businessinsider.com","6SQ66tyZN" },
            new string[] { "Vin","Jugging","vjugging8","vjugging8@ucsd.edu","fFWXWM" },
            new string[] { "Fernande","Devinn","fdevinn9","fdevinn9@oracle.com","SrRGlHyXHAhX" },
            new string[] { "Edik","Playden","eplaydena","eplaydena@themeforest.net","c2FQKojMqf" },
            new string[] { "Adrian","Carrodus","acarrodusb","acarrodusb@hhs.gov","hGnXO6rmWRl" },
            new string[] { "Dieter","Cartmill","dcartmillc","dcartmillc@ovh.net","Mz3nmJr1gQV" },
            new string[] { "Bern","Manby","bmanbyd","bmanbyd@alibaba.com","ibBD2OgcG3y" },
            new string[] { "Robbyn","Genders","rgenderse","rgenderse@1688.com","gFkCfReiSx" },
            new string[] { "Yale","Weddell","yweddellf","yweddellf@yellowpages.com","9vPTMs" },
            new string[] { "Reginauld","Duckit","rduckitg","rduckitg@abc.net.au","HSNOlM5WsZB" },
            new string[] { "Jerome","Mayward","jmaywardh","jmaywardh@globo.com","2HgRn9yI2lr" },
            new string[] { "Karoline","Burnet","kburneti","kburneti@weather.com","CoJlyrmzU" },
            new string[] { "Farly","Lally","flallyj","flallyj@apache.org","i5i5p221ic" },
            new string[] { "Artair","Astall","aastallk","aastallk@yandex.ru","TYrFuELgX7" },
            new string[] { "Jojo","Harwick","jharwickl","jharwickl@trellian.com","UWgmvMpl5p5Y" },
            new string[] { "Mehetabel","Klugel","mklugelm","mklugelm@umn.edu","KMlxzieTPK" },
            new string[] { "Kelly","Lyburn","klyburnn","klyburnn@theguardian.com","qwFtxQ31P" },
            new string[] { "Mercie","Fedorchenko","mfedorchenkoo","mfedorchenkoo@amazon.co.uk","ZARPrIC" },
            new string[] { "Dill","Haldenby","dhaldenbyp","dhaldenbyp@t-online.de","iipq3MPxxN" },
            new string[] { "Anatola","Andrasch","aandraschq","aandraschq@wikia.com","3mHBCGe6FUhw" },
            new string[] { "Alexis","Dominetti","adominettir","adominettir@uol.com.br","rx2VI49L" },
            new string[] { "Etienne","Jahner","ejahners","ejahners@taobao.com","abooQMby" },
            new string[] { "Pietrek","Gurner","pgurnert","pgurnert@mlb.com","D7aHo8k" },
            new string[] { "Lydia","Caesmans","lcaesmansu","lcaesmansu@posterous.com","eJ1cXCq0" },
            new string[] { "Ava","Shard","ashardv","ashardv@timesonline.co.uk","6spOhe7eJ" },
            new string[] { "Zed","Vasyunin","zvasyuninw","zvasyuninw@cdc.gov","qeqcEPte54L5" },
            new string[] { "Chrissie","D'Aubney","cdaubneyx","cdaubneyx@weather.com","7qxBItsk" },
            new string[] { "Roley","Rosborough","rrosboroughy","rrosboroughy@naver.com","yBkkelv4" },
            new string[] { "Fawnia","Larmuth","flarmuthz","flarmuthz@cloudflare.com","V7xj1Ly" },
            new string[] { "Jerrilee","Wrintmore","jwrintmore10","jwrintmore10@facebook.com","kD0qSfkqLn" },
            new string[] { "Diena","Romney","dromney11","dromney11@netlog.com","XoiLhK5vZWor" },
            new string[] { "Candra","Meek","cmeek12","cmeek12@japanpost.jp","3na3VqEJi" },
            new string[] { "Jacobo","Buglar","jbuglar13","jbuglar13@bandcamp.com","UZ2DjEnJZVy" },
            new string[] { "Neddy","Knibbs","nknibbs14","nknibbs14@woothemes.com","OhfdZv" },
            new string[] { "Kirk","Manz","kmanz15","kmanz15@themeforest.net","7HBatFEh" },
            new string[] { "Payton","Jaskiewicz","pjaskiewicz16","pjaskiewicz16@answers.com","oY9im2" },
            new string[] { "Darbee","Astin","dastin17","dastin17@reddit.com","yr5VeeJz" },
            new string[] { "Martelle","Radbourn","mradbourn18","mradbourn18@microsoft.com","R09DEReRo9" }
        };

        private static string[] administrator = new string [] {
            "Admin","Admin","admin","admin@microsoft.com","admin12345", "muski"
        };

        private static bool addUser ( string[] row, UserManager<User> userManager, string role ) {
            string firstName = row[0];
            string lastName = row[1];
            string username = row[2];
            string email = row[3];
            string password = row[4];
            string gender = row[5];

            if ( userManager.FindByNameAsync ( username ).Result != null ) {
                return false;
            }

            User user = new User ( ) {
                firstName = firstName,
                lastName = lastName,
                UserName = username,
                Email = email,
                gender = gender
            };

            IdentityResult result = userManager.CreateAsync ( user, password ).Result;
            if ( !result.Succeeded ) {
                return false;
            }

            result = userManager.AddToRoleAsync ( user, role ).Result;

            if ( !result.Succeeded ) {
                return false;
            }

            return true;
        }

        public static void initialize  ( UserManager<User> userManager ) {
            /*foreach ( string[] row in UserInitializer.users ) {
                bool result = addUser ( row, userManager, IdentityRoleConfiguration.Roles.user.Name );
                if ( !result ) {
                    return;
                }
            }*/

            addUser ( UserInitializer.administrator, userManager, IdentityRoleConfiguration.Roles.admin.Name );
        }
    }
}