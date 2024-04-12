//using google.apis.auth.oauth2;
//using google.apis;
//using system.net.http;
//using firebaseadmin;
//using microsoft.identity.client.platforms.features.desktopos.kerberos;
//using firebaseadmin.messaging;
//using org.bouncycastle.asn1;
//using system.reflection;

//namespace appify.web.api
//{
//    public partial class pushnotification
//    {
//        //public async void sendnotification(string payload) {
//        //    var firebasesettingsjson = await file.readalltextasync(@"./link/to/my-project-123345-e12345.json");
//        //    var fcm = new firebasesender(firebasesettingsjson, system.net.http.httpclient);
//        //    await fcm.sendasync(payload);

//        //}

//        public void sendnotification(string title, string messagetext) {

//            var rootpath = system.io.path.getdirectoryname(assembly.getentryassembly().location);

//            firebaseapp.create(new appoptions() { credential = googlecredential.fromfile(@"d:\project\appify\appify.web.api\keys\appify-android-gcp-firebase-adminsdk-m95az-295f168b98.json") });

//            var targettoken = "eeucjbol9e5qj1k8eyprtt:apa91bg09g-qwrdwqtfjscycuyykcyqx8sld7omqmr1qutgbxahyayzwlxixlokpiii2az5juy1l9umwsznpxg450k3jgb2yjqv93egssaonlm9dxhil1ommdttmh6hofkfxovimerm1";

//            var message = new message();

//            notification notification = new notification();
//            notification.title = title;
//            notification.body = messagetext;

//            dictionary<string,string> data = new dictionary<string,string>();
//            data.add(title, messagetext);

//            message.notification = notification;
//            message.data = data;
//            message.token = targettoken;

//            string response = firebasemessaging.defaultinstance.sendasync(message).result;
//        }
//    }
//}
