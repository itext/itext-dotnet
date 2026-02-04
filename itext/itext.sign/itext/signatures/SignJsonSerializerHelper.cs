using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iText.Commons.Bouncycastle.Cert;
using iText.Commons.Json;
using iText.Commons.Utils;
using iText.Kernel.Exceptions;
using iText.Signatures.Validation.Lotl.Criteria;

namespace iText.Signatures {
    /// <summary>Helper class for JSON AST serialization/deserialization.</summary>
    public sealed class SignJsonSerializerHelper {
        private const String JSON_KEY_BASE64_ENCODED = "base64Encoded";

        private const String JSON_KEY_CRITERIA_LIST = "criteriaList";

        private const String JSON_KEY_CRITERIAS = "criterias";

        private const String JSON_KEY_CRITERIA_ASSERT_VALUE = "criteriaAssertValue";

        private const String JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA = "certSubjectDNAttributeCriteria";

        private const String JSON_KEY_CRITERIA_REQUIRED_ATTRIBUTE_IDS = "requiredAttributeIDs";

        private const String JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA = "extendedKeyUsageCriteria";

        private const String JSON_KEY_CRITERIA_REQUIRED_EXTENDED_KEY_USAGES = "requiredExtendedKeyUsages";

        private const String JSON_KEY_CRITERIA_POLICY_SET_CRITERIA = "policySetCriteria";

        private const String JSON_KEY_CRITERIA_REQUIRED_POLICY_IDS = "requiredPolicyIDs";

        private const String JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA = "keyUsageCriteria";

        private const String JSON_KEY_CRITERIA_KEY_USAGE_BITS = "requiredKeyUsageBits";

        private SignJsonSerializerHelper() {
        }

        // Empty.
        /// <summary>
        /// Serializes
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// object to JSON AST.
        /// </summary>
        /// <param name="certificate">
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// object to serialize
        /// </param>
        /// <returns>serialized certificate as JSON AST</returns>
        public static JsonValue SerializeCertificate(IX509Certificate certificate) {
            if (certificate == null) {
                return JsonNull.JSON_NULL;
            }
            byte[] encoded;
            try {
                encoded = certificate.GetEncoded();
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
            String base64Encoded = Convert.ToBase64String(encoded);
            JsonObject certificateJson = new JsonObject();
            certificateJson.Add(JSON_KEY_BASE64_ENCODED, new JsonString(base64Encoded));
            return certificateJson;
        }

        /// <summary>Deserializes JSON AST object into certificate.</summary>
        /// <param name="certificateJson">
        /// 
        /// <see cref="iText.Commons.Json.JsonValue"/>
        /// JSON AST to deserialize
        /// </param>
        /// <returns>
        /// 
        /// <see cref="iText.Commons.Bouncycastle.Cert.IX509Certificate"/>
        /// deserialized certificate
        /// </returns>
        public static IX509Certificate DeserializeCertificate(JsonValue certificateJson) {
            if (certificateJson == JsonNull.JSON_NULL) {
                return null;
            }
            String base64Encoded = ((JsonString)((JsonObject)certificateJson).GetField(JSON_KEY_BASE64_ENCODED)).GetValue
                ();
            byte[] decoded = Convert.FromBase64String(base64Encoded);
            try {
                return (IX509Certificate)CertificateUtil.GenerateCertificate(new MemoryStream(decoded));
            }
            catch (Exception e) {
                throw new PdfException(e);
            }
        }

        /// <summary>
        /// Serializes
        /// <see cref="iText.Signatures.Validation.Lotl.Criteria.CriteriaList"/>
        /// object to JSON AST.
        /// </summary>
        /// <param name="criteriaList">
        /// 
        /// <see cref="iText.Signatures.Validation.Lotl.Criteria.CriteriaList"/>
        /// to serialize
        /// </param>
        /// <returns>
        /// serialized
        /// <see cref="iText.Signatures.Validation.Lotl.Criteria.CriteriaList"/>
        /// as JSON AST
        /// </returns>
        public static JsonValue SerializeCriteriaList(CriteriaList criteriaList) {
            JsonObject criteriaListJson = new JsonObject();
            criteriaListJson.Add(JSON_KEY_CRITERIA_ASSERT_VALUE, new JsonString(criteriaList.GetAssertValue()));
            JsonArray criteriasJson = new JsonArray();
            foreach (iText.Signatures.Validation.Lotl.Criteria.Criteria criteria in criteriaList.GetCriteriaList()) {
                if (criteria is CriteriaList) {
                    JsonObject innerCriteriaListJson = new JsonObject();
                    innerCriteriaListJson.Add(JSON_KEY_CRITERIA_LIST, SerializeCriteriaList((CriteriaList)criteria));
                    criteriasJson.Add(innerCriteriaListJson);
                }
                else {
                    if (criteria is CertSubjectDNAttributeCriteria) {
                        JsonObject certSubjectDNAttributeCriteriaJson = new JsonObject();
                        JsonObject criteriaRequiredAttributeIdsJson = new JsonObject();
                        criteriaRequiredAttributeIdsJson.Add(JSON_KEY_CRITERIA_REQUIRED_ATTRIBUTE_IDS, new JsonArray(((CertSubjectDNAttributeCriteria
                            )criteria).GetRequiredAttributeIds().Select((requiredAttributeId) => (JsonValue)new JsonString(requiredAttributeId
                            )).ToList()));
                        certSubjectDNAttributeCriteriaJson.Add(JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA, criteriaRequiredAttributeIdsJson
                            );
                        criteriasJson.Add(certSubjectDNAttributeCriteriaJson);
                    }
                    else {
                        if (criteria is ExtendedKeyUsageCriteria) {
                            JsonObject extendedKeyUsageCriteriaJson = new JsonObject();
                            JsonObject criteriaRequiredExtendedKeyUsageJson = new JsonObject();
                            criteriaRequiredExtendedKeyUsageJson.Add(JSON_KEY_CRITERIA_REQUIRED_EXTENDED_KEY_USAGES, new JsonArray(((ExtendedKeyUsageCriteria
                                )criteria).GetRequiredExtendedKeyUsages().Select((requiredExtendedKeyUsage) => (JsonValue)new JsonString
                                (requiredExtendedKeyUsage)).ToList()));
                            extendedKeyUsageCriteriaJson.Add(JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA, criteriaRequiredExtendedKeyUsageJson
                                );
                            criteriasJson.Add(extendedKeyUsageCriteriaJson);
                        }
                        else {
                            if (criteria is KeyUsageCriteria) {
                                IList<String> keyUsages = new List<String>();
                                foreach (bool? keyUsage in ((KeyUsageCriteria)criteria).GetKeyUsageBits()) {
                                    if (keyUsage == null) {
                                        keyUsages.Add("null");
                                    }
                                    else {
                                        keyUsages.Add((bool)keyUsage ? "true" : "false");
                                    }
                                }
                                JsonObject keyUsageCriteriaJson = new JsonObject();
                                JsonObject criteriaRequiredKeyUsageJson = new JsonObject();
                                criteriaRequiredKeyUsageJson.Add(JSON_KEY_CRITERIA_KEY_USAGE_BITS, new JsonArray(keyUsages.Select((requiredExtendedKeyUsage
                                    ) => (JsonValue)new JsonString(requiredExtendedKeyUsage)).ToList()));
                                keyUsageCriteriaJson.Add(JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA, criteriaRequiredKeyUsageJson);
                                criteriasJson.Add(keyUsageCriteriaJson);
                            }
                            else {
                                if (criteria is PolicySetCriteria) {
                                    JsonObject policySetCriteriaJson = new JsonObject();
                                    JsonObject criteriaRequiredPolicyIdsJson = new JsonObject();
                                    criteriaRequiredPolicyIdsJson.Add(JSON_KEY_CRITERIA_REQUIRED_POLICY_IDS, new JsonArray(((PolicySetCriteria
                                        )criteria).GetRequiredPolicyIds().Select((requiredPolicyId) => (JsonValue)new JsonString(requiredPolicyId
                                        )).ToList()));
                                    policySetCriteriaJson.Add(JSON_KEY_CRITERIA_POLICY_SET_CRITERIA, criteriaRequiredPolicyIdsJson);
                                    criteriasJson.Add(policySetCriteriaJson);
                                }
                            }
                        }
                    }
                }
            }
            criteriaListJson.Add(JSON_KEY_CRITERIAS, criteriasJson);
            return criteriaListJson;
        }

        /// <summary>
        /// Deserializes JSON AST object in
        /// <see cref="iText.Signatures.Validation.Lotl.Criteria.CriteriaList"/>.
        /// </summary>
        /// <param name="criteriaListJson">
        /// 
        /// <see cref="iText.Commons.Json.JsonObject"/>
        /// to create
        /// <see cref="iText.Signatures.Validation.Lotl.Criteria.CriteriaList"/>
        /// from
        /// </param>
        /// <returns>
        /// deserialized
        /// <see cref="iText.Signatures.Validation.Lotl.Criteria.CriteriaList"/>
        /// </returns>
        public static CriteriaList DeserializeCriteriaList(JsonObject criteriaListJson) {
            JsonString assertValueJson = (JsonString)criteriaListJson.GetField(JSON_KEY_CRITERIA_ASSERT_VALUE);
            CriteriaList criteriaList = new CriteriaList(assertValueJson.GetValue());
            JsonArray criteriasJson = (JsonArray)criteriaListJson.GetField(JSON_KEY_CRITERIAS);
            foreach (JsonValue criteriaJson in criteriasJson.GetValues()) {
                JsonObject criteriaJsonObject = (JsonObject)criteriaJson;
                if (criteriaJsonObject.GetField(JSON_KEY_CRITERIA_LIST) != null) {
                    iText.Signatures.Validation.Lotl.Criteria.Criteria innerCriteriaList = DeserializeCriteriaList((JsonObject
                        )criteriaJsonObject.GetField(JSON_KEY_CRITERIA_LIST));
                    criteriaList.AddCriteria(innerCriteriaList);
                    continue;
                }
                if (criteriaJsonObject.GetField(JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA) != null) {
                    CertSubjectDNAttributeCriteria criteriaFromJson = new CertSubjectDNAttributeCriteria();
                    JsonArray requiredAttributeIDsJson = (JsonArray)((JsonObject)criteriaJsonObject.GetField(JSON_KEY_CRITERIA_CERT_SUBJECT_DN_ATTRIBUTE_CRITERIA
                        )).GetField(JSON_KEY_CRITERIA_REQUIRED_ATTRIBUTE_IDS);
                    foreach (JsonValue attributeIDJson in requiredAttributeIDsJson.GetValues()) {
                        criteriaFromJson.AddRequiredAttributeId(((JsonString)attributeIDJson).GetValue());
                    }
                    criteriaList.AddCriteria(criteriaFromJson);
                    continue;
                }
                if (criteriaJsonObject.GetField(JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA) != null) {
                    ExtendedKeyUsageCriteria criteriaFromJson = new ExtendedKeyUsageCriteria();
                    JsonArray requiredExtendedKeyUsagesJson = (JsonArray)((JsonObject)criteriaJsonObject.GetField(JSON_KEY_CRITERIA_EXTENDED_KEY_USAGE_CRITERIA
                        )).GetField(JSON_KEY_CRITERIA_REQUIRED_EXTENDED_KEY_USAGES);
                    foreach (JsonValue extendedKeyUsageJson in requiredExtendedKeyUsagesJson.GetValues()) {
                        criteriaFromJson.AddRequiredExtendedKeyUsage(((JsonString)extendedKeyUsageJson).GetValue());
                    }
                    criteriaList.AddCriteria(criteriaFromJson);
                    continue;
                }
                if (criteriaJsonObject.GetField(JSON_KEY_CRITERIA_POLICY_SET_CRITERIA) != null) {
                    PolicySetCriteria criteriaFromJson = new PolicySetCriteria();
                    JsonArray requiredPolicyIDsJson = (JsonArray)((JsonObject)criteriaJsonObject.GetField(JSON_KEY_CRITERIA_POLICY_SET_CRITERIA
                        )).GetField(JSON_KEY_CRITERIA_REQUIRED_POLICY_IDS);
                    foreach (JsonValue policyIdJson in requiredPolicyIDsJson.GetValues()) {
                        criteriaFromJson.AddRequiredPolicyId(((JsonString)policyIdJson).GetValue());
                    }
                    criteriaList.AddCriteria(criteriaFromJson);
                    continue;
                }
                if (criteriaJsonObject.GetField(JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA) != null) {
                    KeyUsageCriteria criteriaFromJson = new KeyUsageCriteria();
                    JsonArray requiredKeyUsageBitsJson = (JsonArray)((JsonObject)criteriaJsonObject.GetField(JSON_KEY_CRITERIA_KEY_USAGE_CRITERIA
                        )).GetField(JSON_KEY_CRITERIA_KEY_USAGE_BITS);
                    int counter = 0;
                    foreach (JsonValue keyUsageBitsJson in requiredKeyUsageBitsJson.GetValues()) {
                        String text = ((JsonString)keyUsageBitsJson).GetValue();
                        criteriaFromJson.GetKeyUsageBits()[counter] = "null".Equals(text) ? null : (bool?)"true".Equals(text);
                        counter++;
                    }
                    criteriaList.AddCriteria(criteriaFromJson);
                }
            }
            return criteriaList;
        }
    }
}
