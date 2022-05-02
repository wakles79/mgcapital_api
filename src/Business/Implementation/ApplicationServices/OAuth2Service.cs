using MGCap.Business.Abstract.ApplicationServices;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace MGCap.Business.Implementation.ApplicationServices
{
    public class OAuth2Service: IOAuth2Service
    {
        UserCredential credential { get; set; }
        public UserCredential GetUserCredential()
        {
            string clientId = "24507622230-r3ni81nvs5fakcjv5sorad123pbck98q.apps.googleusercontent.com";
            string clientSecret = "TP3DqJ30unjdhF25j9IxdmI9";
            string[] scopes = { GmailService.ScopeConstants.MailGoogleCom };
            var credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId=clientId,
                    ClientSecret=clientSecret
                },
                scopes,
                "user",
                CancellationToken.None).Result;
            return credentials;
            //try
            //{
            //    ClientSecretsJson clientSecrets = new ClientSecretsJson();
            //    //Stream stream = new MemoryStream(ObjectToByteArray(clientSecrets));
            //    //BinaryFormatter formater = new BinaryFormatter();
            //    //formater.Serialize(stream, clientSecrets);
            //    //String FilePath = Path.Combine("~/", "DriveServiceCredentials.json");
            //    //BinaryFormatter bf = new BinaryFormatter();
            //    byte[] jsonBytes = ObjectToByteArray(clientSecrets);
            //    using (MemoryStream ms = new MemoryStream(jsonBytes))
            //    {
            //        //bf.Serialize(ms, clientSecrets);
            //        this.credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(ms).Secrets,
            //            new[] { GmailService.ScopeConstants.MailGoogleCom },
            //            "user",
            //            System.Threading.CancellationToken.None);
            //    }
            //    return credential;
            //}
            //catch(SerializationException e)
            //{
            //    string error = e.Message;
            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    string error = ex.Message;
            //    return null;
            //}

        }

        byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }

    [Serializable]
    class ClientSecretsJson
    {
        public string Type = "service_account";
        public string Project_id = "mailtests-323317";
        public string Private_key_id = "5d50d60f1aeff4c51f24a7efa9c264726bf0c70b";
        public string Private_key = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC2N0BcFGiSvScf\nsPTke6FSg4yz1ma+MBPck95NSXw3URtAomixTQt/EUzKCOkhfqMheUZ+soh7gV9C\nz438xGp8Eh6BppOenhj04K15U6kEQDyayC5hMycg8/okYHSve3S3mdeB3cJOxVBh\n6oiCIpjMu6KG2MWLOSESYo8qwd2sNaknkubUPUGkYXT7Ynk6rZXRiVLuvesY6zFL\nBa+ljesgvzHc8UMOPu1Od1cDYI8I+RF7UahK3JYlu+TUIBtp5e0l/caDqNM54fCU\nlr9r0K4xfpQCN2Jbd/6B0TL4/nEBEsffhGTrksTL81W5VpMDgdYvRJjNHrpNj02p\nez0Gdpp/AgMBAAECggEACH1jiMbEpo2cnIvkpR6jghIyBJEJQXRBAKZ9FZHGcVdc\nkzLd+ODTl8yJDOLEZfSeCim54J5V4JdDBnxgK7ZFFdnMRRMrTpkkJYiwXHZK6jFK\nAZYxHwNAn/buITLDTQfOBN3snhbkfkIQQDequ5F8GBHn/SttbNKxGUexTvcwal0T\naesdlQa/4NrAtJf5qa3G+u67VPN3xDF39P5mZM0eaWii3lJQDFbVVzpeEAuVR4ln\nz8ZgU2iOUMuUBUWR2RXVUREVdDbIhmXNl6wZpTCBCQ/pHPh2aqNASm8VRfkDbc8i\nEQBRTXM5XhIdVUoS35VkLi350oBvmZCjNLglAAz3BQKBgQDzyheGS1e2odi3htFw\nkixPGeW9BSfybnY3DBWfRweQUwwi9Ew5T/NNQe32Z1bw1+RUTFt05Gw3cRF+r945\naoQv2CrqE0DSJHyOl3ct59WzI6br3xRBZOvoTpsdNEp3yo/SKJOOvBiTs+8aO8lO\ntKadUfrmh2k6xyXV3z2wTITZCwKBgQC/V6cZQqeCvjym37JWh/BZOl3vj+VOVcK5\nkVzafH71hk6jg/tRib0ghqEpvqaJLiGUNp3+maHQ0m/lNgYDJIHGHYqMhCwU8CT0\nNGYCAI0s+WPyAjlwbuzCGESSWOerAbruqTN2a3YZV8bprCG7DTsRqgH4/eFA0GEa\ncrnSS7M03QKBgAn92GT6pYeorncnIWJZu1Mqno3WrkJzYIj1B9XVVWwG21J7bLxU\nmIYsE5KrvZZ1YrhJwN3TltPW4H6uuo7j3LMRSwD49Qkn0asObiYFgG6tIWQ9alkb\nx8dnoVfbEspCFQ7p9tI8x00WbNBIbwG4ybNc7smP9zIfg192nNKi8DbtAoGBAKyj\nrm3ZOQB7GohKD0OJta8cPYMXpwKN2HrMECZF69slijB4tbn0+AKv1huvG91sFPoA\naX89KMYcCL+bhcMyHjEdmFN+MbPWlNENsfgefUJV5fqGTUMHonDhqoUM2EHv9rS9\nT2SKQu1MyHkvSOPXZTNj3BHHJ4TuPp65YrW+fXu1AoGAUtNUAIo3A1PMHmUsBWCO\n1cwG654jrDmHrePmb4MaE0ipdeuOcK37llwRavomYd6HgHoyHI2UvR9VFY/tJq90\nDJ4LvzSBh0vOp5cb1xPf/JFvI92iufn9qbpnJl1i59SmYtSqCC8HCU3Ape60NPF9\n7QxlGAbI4/NFrMyYSWb3ioQ=\n-----END PRIVATE KEY-----\n";
        public string Client_email = "pubsubsystem@mailtests-323317.iam.gserviceaccount.com";
        public string Client_id = "111660851373284226946";
        public string Auth_uri = "https://accounts.google.com/o/oauth2/auth";
        public string Token_uri = "https://oauth2.googleapis.com/token";
        public string Auth_provider_x509_cert_url = "https://www.googleapis.com/oauth2/v1/certs";
        public string Client_x509_cert_url = "https://www.googleapis.com/robot/v1/metadata/x509/pubsubsystem%40mailtests-323317.iam.gserviceaccount.com";
    }
}
