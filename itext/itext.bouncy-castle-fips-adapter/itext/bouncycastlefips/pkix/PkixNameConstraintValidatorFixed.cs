/*
This file is part of bcpkix-fips, see NOTICE.txt in the root of the repository.
It may contain modifications beyond the original version.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X500;
using Org.BouncyCastle.Asn1.X500.Style;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkix;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Collections;

namespace iText.Bouncycastlefips.Pkix
{
  // TODO DEVSIX-9827 Remove this class and use original Org.BouncyCastle.Pkix.PkixNameConstraintValidator instead.
  public class PkixNameConstraintValidatorFixed
  {
    private static readonly DerObjectIdentifier SerialNumberOid = Rfc4519Style.serialNumber;
    private ISet<Asn1Sequence> excludedSubtreesDN = (ISet<Asn1Sequence>) new HashSet<Asn1Sequence>();
    private ISet<string> excludedSubtreesDNS = (ISet<string>) new HashSet<string>();
    private ISet<string> excludedSubtreesEmail = (ISet<string>) new HashSet<string>();
    private ISet<string> excludedSubtreesURI = (ISet<string>) new HashSet<string>();
    private ISet<byte[]> excludedSubtreesIP = (ISet<byte[]>) new HashSet<byte[]>();
    private ISet<Asn1Sequence> permittedSubtreesDN;
    private ISet<string> permittedSubtreesDNS;
    private ISet<string> permittedSubtreesEmail;
    private ISet<string> permittedSubtreesURI;
    private ISet<byte[]> permittedSubtreesIP;

    private static bool WithinDNSubtree(Asn1Sequence dns, Asn1Sequence subtree)
    {
      if (subtree.Count < 1 || subtree.Count > dns.Count)
        return false;
      int num = 0;
      Rdn instance1 = Rdn.GetInstance((object) subtree[0]);
      for (int index = 0; index < dns.Count; ++index)
      {
        num = index;
        Rdn instance2 = Rdn.GetInstance((object) dns[index]);
        if (IetfUtils.RdnAreEqual(instance1, instance2))
          break;
      }

      if (subtree.Count > dns.Count - num)
        return false;
      for (int index = 0; index < subtree.Count; ++index)
      {
        Rdn instance3 = Rdn.GetInstance((object) subtree[index]);
        Rdn instance4 = Rdn.GetInstance((object) dns[num + index]);
        if (instance3.Count == 1 && instance4.Count == 1 &&
            PkixNameConstraintValidatorFixed.SerialNumberOid.Equals((object) instance3.First.Type) &&
            PkixNameConstraintValidatorFixed.SerialNumberOid.Equals((object) instance4.First.Type))
        {
          if (!CultureInfo.InvariantCulture.CompareInfo.IsPrefix(instance4.First.Value.ToString(), instance3.First.Value.ToString(), CompareOptions.Ordinal))
            return false;
        }
        else if (!IetfUtils.RdnAreEqual(instance3, instance4))
          return false;
      }

      return true;
    }

    public void CheckPermittedDN(Asn1Sequence dns) => this.CheckPermittedDN(this.permittedSubtreesDN, dns);

    public void CheckExcludedDN(Asn1Sequence dns) => this.CheckExcludedDN(this.excludedSubtreesDN, dns);

    private void CheckPermittedDN(ISet<Asn1Sequence> permitted, Asn1Sequence dns)
    {
      if (permitted != null && (permitted.Count != 0 || dns.Count != 0))
      {
        IEnumerator<Asn1Sequence> enumerator = permitted.GetEnumerator();
        while (enumerator.MoveNext())
        {
          Asn1Sequence current = enumerator.Current;
          if (PkixNameConstraintValidatorFixed.WithinDNSubtree(dns, current))
            return;
        }

        throw new PkixNameConstraintValidatorException("Subject distinguished name is not from a permitted subtree");
      }
    }

    private void CheckExcludedDN(ISet<Asn1Sequence> excluded, Asn1Sequence dns)
    {
      if (excluded.Count == 0)
        return;
      IEnumerator enumerator = (IEnumerator) excluded.GetEnumerator();
      while (enumerator.MoveNext())
      {
        Asn1Sequence current = (Asn1Sequence) enumerator.Current;
        if (PkixNameConstraintValidatorFixed.WithinDNSubtree(dns, current))
          throw new PkixNameConstraintValidatorException("Subject distinguished name is from an excluded subtree");
      }
    }

    private ISet<Asn1Sequence> IntersectDN(
      ISet<Asn1Sequence> permitted,
      ISet<GeneralSubtree> dns)
    {
      ISet<Asn1Sequence> asn1SequenceSet = (ISet<Asn1Sequence>) new HashSet<Asn1Sequence>();
      IEnumerator enumerator1 = (IEnumerator) dns.GetEnumerator();
      while (enumerator1.MoveNext())
      {
        Asn1Sequence instance =
          Asn1Sequence.GetInstance((object) ((GeneralSubtree) enumerator1.Current).Base.Name.ToAsn1Object());
        if (permitted == null)
        {
          if (instance != null)
            asn1SequenceSet.Add(instance);
        }
        else
        {
          IEnumerator<Asn1Sequence> enumerator2 = permitted.GetEnumerator();
          while (enumerator2.MoveNext())
          {
            Asn1Sequence current = enumerator2.Current;
            if (PkixNameConstraintValidatorFixed.WithinDNSubtree(instance, current))
              asn1SequenceSet.Add(instance);
            else if (PkixNameConstraintValidatorFixed.WithinDNSubtree(current, instance))
              asn1SequenceSet.Add(current);
          }
        }
      }

      return asn1SequenceSet;
    }

    private ISet<Asn1Sequence> UnionDN(ISet<Asn1Sequence> excluded, Asn1Sequence dn)
    {
      if (excluded.Count == 0)
      {
        if (dn == null)
          return excluded;
        excluded.Add(dn);
        return excluded;
      }

      ISet<Asn1Sequence> asn1SequenceSet = (ISet<Asn1Sequence>) new HashSet<Asn1Sequence>();
      IEnumerator<Asn1Sequence> enumerator = excluded.GetEnumerator();
      while (enumerator.MoveNext())
      {
        Asn1Sequence current = enumerator.Current;
        if (PkixNameConstraintValidatorFixed.WithinDNSubtree(dn, current))
          asn1SequenceSet.Add(current);
        else if (PkixNameConstraintValidatorFixed.WithinDNSubtree(current, dn))
        {
          asn1SequenceSet.Add(dn);
        }
        else
        {
          asn1SequenceSet.Add(current);
          asn1SequenceSet.Add(dn);
        }
      }

      return asn1SequenceSet;
    }

    private ISet<string> IntersectEmail(ISet<string> permitted, ISet<GeneralSubtree> emails)
    {
      ISet<string> intersect = (ISet<string>) new HashSet<string>();
      IEnumerator<GeneralSubtree> enumerator1 = emails.GetEnumerator();
      while (enumerator1.MoveNext())
      {
        string nameAsString = this.ExtractNameAsString(enumerator1.Current.Base);
        if (permitted == null)
        {
          if (nameAsString != null)
            intersect.Add(nameAsString);
        }
        else
        {
          IEnumerator<string> enumerator2 = permitted.GetEnumerator();
          while (enumerator2.MoveNext())
          {
            string current = enumerator2.Current;
            this.intersectEmail(nameAsString, current, intersect);
          }
        }
      }

      return intersect;
    }

    private ISet<string> UnionEmail(ISet<string> excluded, string email)
    {
      if (excluded.Count == 0)
      {
        if (email == null)
          return excluded;
        excluded.Add(email);
        return excluded;
      }

      ISet<string> union = (ISet<string>) new HashSet<string>();
      IEnumerator enumerator = (IEnumerator) excluded.GetEnumerator();
      while (enumerator.MoveNext())
        this.unionEmail((string) enumerator.Current, email, union);
      return union;
    }

    private ISet<byte[]> IntersectIP(ISet<byte[]> permitted, ISet ips)
    {
      ISet<byte[]> to = (ISet<byte[]>) new HashSet<byte[]>();
      foreach (GeneralSubtree ip in (IEnumerable) ips)
      {
        byte[] octets = Asn1OctetString.GetInstance((object) ip.Base.Name).GetOctets();
        if (permitted == null)
        {
          if (octets != null)
            to.Add(octets);
        }
        else
        {
          IEnumerator enumerator = (IEnumerator) permitted.GetEnumerator();
          while (enumerator.MoveNext())
          {
            byte[] current = (byte[]) enumerator.Current;
            this.cpy<byte[]>(to, (IEnumerable<byte[]>) this.IntersectIPRange(current, octets));
          }
        }
      }

      return to;
    }

    private ISet<byte[]> UnionIP(ISet<byte[]> excluded, byte[] ip)
    {
      if (excluded.Count == 0)
      {
        if (ip == null)
          return excluded;
        excluded.Add(ip);
        return excluded;
      }

      HashSet<byte[]> to = new HashSet<byte[]>();
      IEnumerator<byte[]> enumerator = excluded.GetEnumerator();
      while (enumerator.MoveNext())
      {
        byte[] current = enumerator.Current;
        this.cpy<byte[]>((ISet<byte[]>) to, (IEnumerable<byte[]>) this.UnionIPRange(current, ip));
      }

      return (ISet<byte[]>) to;
    }

    private void cpy<T>(ISet<T> to, IEnumerable<T> from)
    {
      IEnumerator<T> enumerator = from.GetEnumerator();
      while (enumerator.MoveNext())
        to.Add(enumerator.Current);
    }

    private ISet<byte[]> UnionIPRange(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
    {
      ISet<byte[]> numArraySet = (ISet<byte[]>) new HashSet<byte[]>();
      if (Arrays.AreEqual(ipWithSubmask1, ipWithSubmask2))
      {
        numArraySet.Add(ipWithSubmask1);
      }
      else
      {
        numArraySet.Add(ipWithSubmask1);
        numArraySet.Add(ipWithSubmask2);
      }

      return numArraySet;
    }

    private ISet<byte[]> IntersectIPRange(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
    {
      if (ipWithSubmask1.Length != ipWithSubmask2.Length)
        return (ISet<byte[]>) new HashSet<byte[]>();
      byte[][] ipsAndSubnetMasks = this.ExtractIPsAndSubnetMasks(ipWithSubmask1, ipWithSubmask2);
      byte[] ip1 = ipsAndSubnetMasks[0];
      byte[] numArray1 = ipsAndSubnetMasks[1];
      byte[] ip2_1 = ipsAndSubnetMasks[2];
      byte[] numArray2 = ipsAndSubnetMasks[3];
      byte[][] numArray3 = this.MinMaxIPs(ip1, numArray1, ip2_1, numArray2);
      byte[] ip2_2 = PkixNameConstraintValidatorFixed.Min(numArray3[1], numArray3[3]);
      if (PkixNameConstraintValidatorFixed.CompareTo(PkixNameConstraintValidatorFixed.Max(numArray3[0], numArray3[2]), ip2_2) ==
          1)
        return (ISet<byte[]>) new HashSet<byte[]>();
      byte[] ip = PkixNameConstraintValidatorFixed.Or(numArray3[0], numArray3[2]);
      byte[] subnetMask = PkixNameConstraintValidatorFixed.Or(numArray1, numArray2);
      HashSet<byte[]> numArraySet = new HashSet<byte[]>();
      numArraySet.Add(this.IpWithSubnetMask(ip, subnetMask));
      return (ISet<byte[]>) numArraySet;
    }

    private byte[] IpWithSubnetMask(byte[] ip, byte[] subnetMask)
    {
      int length = ip.Length;
      byte[] destinationArray = new byte[length * 2];
      Array.Copy((Array) ip, 0, (Array) destinationArray, 0, length);
      Array.Copy((Array) subnetMask, 0, (Array) destinationArray, length, length);
      return destinationArray;
    }

    private byte[][] ExtractIPsAndSubnetMasks(byte[] ipWithSubmask1, byte[] ipWithSubmask2)
    {
      int length = ipWithSubmask1.Length / 2;
      byte[] destinationArray1 = new byte[length];
      byte[] destinationArray2 = new byte[length];
      Array.Copy((Array) ipWithSubmask1, 0, (Array) destinationArray1, 0, length);
      Array.Copy((Array) ipWithSubmask1, length, (Array) destinationArray2, 0, length);
      byte[] destinationArray3 = new byte[length];
      byte[] destinationArray4 = new byte[length];
      Array.Copy((Array) ipWithSubmask2, 0, (Array) destinationArray3, 0, length);
      Array.Copy((Array) ipWithSubmask2, length, (Array) destinationArray4, 0, length);
      return new byte[4][]
      {
        destinationArray1,
        destinationArray2,
        destinationArray3,
        destinationArray4
      };
    }

    private byte[][] MinMaxIPs(byte[] ip1, byte[] subnetmask1, byte[] ip2, byte[] subnetmask2)
    {
      int length = ip1.Length;
      byte[] numArray1 = new byte[length];
      byte[] numArray2 = new byte[length];
      byte[] numArray3 = new byte[length];
      byte[] numArray4 = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        numArray1[index] = (byte) ((uint) ip1[index] & (uint) subnetmask1[index]);
        numArray2[index] = (byte) ((uint) ip1[index] & (uint) subnetmask1[index] | (uint) ~subnetmask1[index]);
        numArray3[index] = (byte) ((uint) ip2[index] & (uint) subnetmask2[index]);
        numArray4[index] = (byte) ((uint) ip2[index] & (uint) subnetmask2[index] | (uint) ~subnetmask2[index]);
      }

      return new byte[4][]
      {
        numArray1,
        numArray2,
        numArray3,
        numArray4
      };
    }

    private void CheckPermittedEmail(ISet<string> permitted, string email)
    {
      if (permitted == null)
        return;
      IEnumerator enumerator = (IEnumerator) permitted.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string current = (string) enumerator.Current;
        if (this.EmailIsConstrained(email, current))
          return;
      }

      if (email.Length != 0 || permitted.Count != 0)
        throw new PkixNameConstraintValidatorException("Subject email address is not from a permitted subtree.");
    }

    private void CheckExcludedEmail(ISet<string> excluded, string email)
    {
      if (excluded.Count == 0)
        return;
      IEnumerator enumerator = (IEnumerator) excluded.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string current = (string) enumerator.Current;
        if (this.EmailIsConstrained(email, current))
          throw new PkixNameConstraintValidatorException("Email address is from an excluded subtree.");
      }
    }

    private void CheckPermittedIP(ISet<byte[]> permitted, byte[] ip)
    {
      if (permitted == null)
        return;
      IEnumerator enumerator = (IEnumerator) permitted.GetEnumerator();
      while (enumerator.MoveNext())
      {
        byte[] current = (byte[]) enumerator.Current;
        if (this.IsIPConstrained(ip, current))
          return;
      }

      if (ip.Length != 0 || permitted.Count != 0)
        throw new PkixNameConstraintValidatorException("IP is not from a permitted subtree.");
    }

    private void checkExcludedIP(ISet<byte[]> excluded, byte[] ip)
    {
      if (excluded.Count > 0)
        return;
      IEnumerator enumerator = (IEnumerator) excluded.GetEnumerator();
      while (enumerator.MoveNext())
      {
        byte[] current = (byte[]) enumerator.Current;
        if (this.IsIPConstrained(ip, current))
          throw new PkixNameConstraintValidatorException("IP is from an excluded subtree.");
      }
    }

    private bool IsIPConstrained(byte[] ip, byte[] constraint)
    {
      int length = ip.Length;
      if (length != constraint.Length / 2)
        return false;
      byte[] destinationArray = new byte[length];
      Array.Copy((Array) constraint, length, (Array) destinationArray, 0, length);
      byte[] a = new byte[length];
      byte[] b = new byte[length];
      for (int index = 0; index < length; ++index)
      {
        a[index] = (byte) ((uint) constraint[index] & (uint) destinationArray[index]);
        b[index] = (byte) ((uint) ip[index] & (uint) destinationArray[index]);
      }

      return Arrays.AreEqual(a, b);
    }

    private bool EmailIsConstrained(string email, string constraint)
    {
      string str = email.Substring(email.IndexOf('@') + 1);
      if (constraint.IndexOf('@') != -1)
      {
        if (PkixCertFunctions.ToUpperInvariant(email).Equals(PkixCertFunctions.ToUpperInvariant(constraint)))
          return true;
      }
      else if (!constraint[0].Equals('.'))
      {
        if (PkixCertFunctions.ToUpperInvariant(str).Equals(PkixCertFunctions.ToUpperInvariant(constraint)))
          return true;
      }
      else if (this.WithinDomain(str, constraint))
        return true;

      return false;
    }

    private bool WithinDomain(string testDomain, string domain)
    {
      string source = domain;
      if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(source, ".", CompareOptions.Ordinal))
        source = source.Substring(1);
      string[] strArray1 = source.Split('.');
      string[] strArray2 = testDomain.Split('.');
      if (strArray2.Length <= strArray1.Length)
        return false;
      int num = strArray2.Length - strArray1.Length;
      for (int index = -1; index < strArray1.Length; ++index)
      {
        if (index == -1)
        {
          if (strArray2[index + num].Equals(""))
            return false;
        }
        else if (!string.Equals(strArray2[index + num], strArray1[index], StringComparison.OrdinalIgnoreCase))
          return false;
      }

      return true;
    }

    private void CheckPermittedDNS(ISet<string> permitted, string dns)
    {
      if (permitted == null)
        return;
      IEnumerator<string> enumerator = permitted.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string current = enumerator.Current;
        if (this.WithinDomain(dns, current) || PkixCertFunctions.ToUpperInvariant(dns)
              .Equals(PkixCertFunctions.ToUpperInvariant(current)))
          return;
      }

      if (dns.Length != 0 || permitted.Count != 0)
        throw new PkixNameConstraintValidatorException("DNS is not from a permitted subtree.");
    }

    private void checkExcludedDNS(ISet<string> excluded, string dns)
    {
      if (excluded.Count == 0)
        return;
      IEnumerator enumerator = (IEnumerator) excluded.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string current = (string) enumerator.Current;
        if (this.WithinDomain(dns, current) || string.Equals(dns, current, StringComparison.OrdinalIgnoreCase))
          throw new PkixNameConstraintValidatorException("DNS is from an excluded subtree.");
      }
    }

    private void unionEmail(string email1, string email2, ISet<string> union)
    {
      if (email1.IndexOf('@') != -1)
      {
        string str = email1.Substring(email1.IndexOf('@') + 1);
        if (email2.IndexOf('@') != -1)
        {
          if (string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
          {
            union.Add(email1);
          }
          else
          {
            union.Add(email1);
            union.Add(email2);
          }
        }
        else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
        {
          if (this.WithinDomain(str, email2))
          {
            union.Add(email2);
          }
          else
          {
            union.Add(email1);
            union.Add(email2);
          }
        }
        else if (string.Equals(str, email2, StringComparison.OrdinalIgnoreCase))
        {
          union.Add(email2);
        }
        else
        {
          union.Add(email1);
          union.Add(email2);
        }
      }
      else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email1, ".", CompareOptions.Ordinal))
      {
        if (email2.IndexOf('@') != -1)
        {
          if (this.WithinDomain(email2.Substring(email1.IndexOf('@') + 1), email1))
          {
            union.Add(email1);
          }
          else
          {
            union.Add(email1);
            union.Add(email2);
          }
        }
        else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
        {
          if (this.WithinDomain(email1, email2) || string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
            union.Add(email2);
          else if (this.WithinDomain(email2, email1))
          {
            union.Add(email1);
          }
          else
          {
            union.Add(email1);
            union.Add(email2);
          }
        }
        else if (this.WithinDomain(email2, email1))
        {
          union.Add(email1);
        }
        else
        {
          union.Add(email1);
          union.Add(email2);
        }
      }
      else if (email2.IndexOf('@') != -1)
      {
        if (string.Equals(email2.Substring(email1.IndexOf('@') + 1), email1, StringComparison.OrdinalIgnoreCase))
        {
          union.Add(email1);
        }
        else
        {
          union.Add(email1);
          union.Add(email2);
        }
      }
      else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
      {
        if (this.WithinDomain(email1, email2))
        {
          union.Add(email2);
        }
        else
        {
          union.Add(email1);
          union.Add(email2);
        }
      }
      else if (string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
      {
        union.Add(email1);
      }
      else
      {
        union.Add(email1);
        union.Add(email2);
      }
    }

    private void unionURI(string email1, string email2, ISet<string> union)
    {
      if (email1.IndexOf('@') != -1)
      {
        string str = email1.Substring(email1.IndexOf('@') + 1);
        if (email2.IndexOf('@') != -1)
        {
          if (string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
          {
            union.Add(email1);
          }
          else
          {
            union.Add(email1);
            union.Add(email2);
          }
        }
        else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
        {
          if (this.WithinDomain(str, email2))
          {
            union.Add(email2);
          }
          else
          {
            union.Add(email1);
            union.Add(email2);
          }
        }
        else if (string.Equals(str, email2, StringComparison.OrdinalIgnoreCase))
        {
          union.Add(email2);
        }
        else
        {
          union.Add(email1);
          union.Add(email2);
        }
      }
      else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email1, ".", CompareOptions.Ordinal))
      {
        if (email2.IndexOf('@') != -1)
        {
          if (this.WithinDomain(email2.Substring(email1.IndexOf('@') + 1), email1))
          {
            union.Add(email1);
          }
          else
          {
            union.Add(email1);
            union.Add(email2);
          }
        }
        else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
        {
          if (this.WithinDomain(email1, email2) || string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
            union.Add(email2);
          else if (this.WithinDomain(email2, email1))
          {
            union.Add(email1);
          }
          else
          {
            union.Add(email1);
            union.Add(email2);
          }
        }
        else if (this.WithinDomain(email2, email1))
        {
          union.Add(email1);
        }
        else
        {
          union.Add(email1);
          union.Add(email2);
        }
      }
      else if (email2.IndexOf('@') != -1)
      {
        if (string.Equals(email2.Substring(email1.IndexOf('@') + 1), email1, StringComparison.OrdinalIgnoreCase))
        {
          union.Add(email1);
        }
        else
        {
          union.Add(email1);
          union.Add(email2);
        }
      }
      else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
      {
        if (this.WithinDomain(email1, email2))
        {
          union.Add(email2);
        }
        else
        {
          union.Add(email1);
          union.Add(email2);
        }
      }
      else if (string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
      {
        union.Add(email1);
      }
      else
      {
        union.Add(email1);
        union.Add(email2);
      }
    }

    private ISet<string> intersectDNS(ISet<string> permitted, ISet<GeneralSubtree> dnss)
    {
      ISet<string> stringSet = (ISet<string>) new HashSet<string>();
      IEnumerator enumerator1 = (IEnumerator) dnss.GetEnumerator();
      while (enumerator1.MoveNext())
      {
        string nameAsString = this.ExtractNameAsString(((GeneralSubtree) enumerator1.Current).Base);
        if (permitted == null)
        {
          if (nameAsString != null)
            stringSet.Add(nameAsString);
        }
        else
        {
          IEnumerator<string> enumerator2 = permitted.GetEnumerator();
          while (enumerator2.MoveNext())
          {
            string current = enumerator2.Current;
            if (this.WithinDomain(current, nameAsString))
              stringSet.Add(current);
            else if (this.WithinDomain(nameAsString, current))
              stringSet.Add(nameAsString);
          }
        }
      }

      return stringSet;
    }

    protected ISet<string> unionDNS(ISet<string> excluded, string dns)
    {
      if (excluded.Count == 0)
      {
        if (dns == null)
          return excluded;
        excluded.Add(dns);
        return excluded;
      }

      ISet<string> stringSet = (ISet<string>) new HashSet<string>();
      IEnumerator<string> enumerator = excluded.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string current = enumerator.Current;
        if (this.WithinDomain(current, dns))
          stringSet.Add(dns);
        else if (this.WithinDomain(dns, current))
        {
          stringSet.Add(current);
        }
        else
        {
          stringSet.Add(current);
          stringSet.Add(dns);
        }
      }

      return stringSet;
    }

    private void intersectEmail(string email1, string email2, ISet<string> intersect)
    {
      if (email1.IndexOf('@') != -1)
      {
        string str = email1.Substring(email1.IndexOf('@') + 1);
        if (email2.IndexOf('@') != -1)
        {
          if (!string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
            return;
          intersect.Add(email1);
        }
        else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
        {
          if (!this.WithinDomain(str, email2))
            return;
          intersect.Add(email1);
        }
        else
        {
          if (!string.Equals(str, email2, StringComparison.OrdinalIgnoreCase))
            return;
          intersect.Add(email1);
        }
      }
      else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email1, ".", CompareOptions.Ordinal))
      {
        if (email2.IndexOf('@') != -1)
        {
          if (!this.WithinDomain(email2.Substring(email1.IndexOf('@') + 1), email1))
            return;
          intersect.Add(email2);
        }
        else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
        {
          if (this.WithinDomain(email1, email2) || string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
          {
            intersect.Add(email1);
          }
          else
          {
            if (!this.WithinDomain(email2, email1))
              return;
            intersect.Add(email2);
          }
        }
        else
        {
          if (!this.WithinDomain(email2, email1))
            return;
          intersect.Add(email2);
        }
      }
      else if (email2.IndexOf('@') != -1)
      {
        if (!string.Equals(email2.Substring(email2.IndexOf('@') + 1), email1, StringComparison.OrdinalIgnoreCase))
          return;
        intersect.Add(email2);
      }
      else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
      {
        if (!this.WithinDomain(email1, email2))
          return;
        intersect.Add(email1);
      }
      else
      {
        if (!string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
          return;
        intersect.Add(email1);
      }
    }

    private void checkExcludedURI(ISet<string> excluded, string uri)
    {
      if (excluded.Count == 0)
        return;
      IEnumerator enumerator = (IEnumerator) excluded.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string current = (string) enumerator.Current;
        if (this.IsUriConstrained(uri, current))
          throw new PkixNameConstraintValidatorException("URI is from an excluded subtree.");
      }
    }

    private ISet<string> intersectURI(ISet<string> permitted, ISet<GeneralSubtree> uris)
    {
      ISet<string> intersect = (ISet<string>) new HashSet<string>();
      IEnumerator enumerator1 = (IEnumerator) uris.GetEnumerator();
      while (enumerator1.MoveNext())
      {
        string nameAsString = this.ExtractNameAsString(((GeneralSubtree) enumerator1.Current).Base);
        if (permitted == null)
        {
          if (nameAsString != null)
            intersect.Add(nameAsString);
        }
        else
        {
          IEnumerator enumerator2 = (IEnumerator) permitted.GetEnumerator();
          while (enumerator2.MoveNext())
            this.intersectURI((string) enumerator2.Current, nameAsString, intersect);
        }
      }

      return intersect;
    }

    private ISet<string> unionURI(ISet<string> excluded, string uri)
    {
      if (excluded.Count == 0)
      {
        if (uri == null)
          return excluded;
        excluded.Add(uri);
        return excluded;
      }

      ISet<string> union = (ISet<string>) new HashSet<string>();
      IEnumerator<string> enumerator = excluded.GetEnumerator();
      while (enumerator.MoveNext())
        this.unionURI(enumerator.Current, uri, union);
      return union;
    }

    private void intersectURI(string email1, string email2, ISet<string> intersect)
    {
      if (email1.IndexOf('@') != -1)
      {
        string str = email1.Substring(email1.IndexOf('@') + 1);
        if (email2.IndexOf('@') != -1)
        {
          if (!string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
            return;
          intersect.Add(email1);
        }
        else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
        {
          if (!this.WithinDomain(str, email2))
            return;
          intersect.Add(email1);
        }
        else
        {
          if (!string.Equals(str, email2, StringComparison.OrdinalIgnoreCase))
            return;
          intersect.Add(email1);
        }
      }
      else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email1, ".", CompareOptions.Ordinal))
      {
        if (email2.IndexOf('@') != -1)
        {
          if (!this.WithinDomain(email2.Substring(email1.IndexOf('@') + 1), email1))
            return;
          intersect.Add(email2);
        }
        else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
        {
          if (this.WithinDomain(email1, email2) || string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
          {
            intersect.Add(email1);
          }
          else
          {
            if (!this.WithinDomain(email2, email1))
              return;
            intersect.Add(email2);
          }
        }
        else
        {
          if (!this.WithinDomain(email2, email1))
            return;
          intersect.Add(email2);
        }
      }
      else if (email2.IndexOf('@') != -1)
      {
        if (!string.Equals(email2.Substring(email2.IndexOf('@') + 1), email1, StringComparison.OrdinalIgnoreCase))
          return;
        intersect.Add(email2);
      }
      else if (CultureInfo.InvariantCulture.CompareInfo.IsPrefix(email2, ".", CompareOptions.Ordinal))
      {
        if (!this.WithinDomain(email1, email2))
          return;
        intersect.Add(email1);
      }
      else
      {
        if (!string.Equals(email1, email2, StringComparison.OrdinalIgnoreCase))
          return;
        intersect.Add(email1);
      }
    }

    private void CheckPermittedURI(ISet<string> permitted, string uri)
    {
      if (permitted == null)
        return;
      IEnumerator<string> enumerator = permitted.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string current = enumerator.Current;
        if (this.IsUriConstrained(uri, current))
          return;
      }

      if (uri.Length != 0 || permitted.Count != 0)
        throw new PkixNameConstraintValidatorException("URI is not from a permitted subtree.");
    }

    private bool IsUriConstrained(string uri, string constraint)
    {
      string hostFromUrl = PkixNameConstraintValidatorFixed.ExtractHostFromURL(uri);
      if (!CultureInfo.InvariantCulture.CompareInfo.IsPrefix(constraint, ".", CompareOptions.Ordinal))
      {
        if (string.Equals(hostFromUrl, constraint, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      else if (this.WithinDomain(hostFromUrl, constraint))
        return true;

      return false;
    }

    private static string ExtractHostFromURL(string url)
    {
      string source = url.Substring(url.IndexOf(':') + 1);
      int num = CultureInfo.InvariantCulture.CompareInfo.IndexOf(source, "//", CompareOptions.Ordinal);
      if (num != -1)
        source = source.Substring(num + 2);
      if (source.LastIndexOf(':') != -1)
        source = source.Substring(0, source.LastIndexOf(':'));
      string str = source.Substring(source.IndexOf(':') + 1);
      string hostFromUrl = str.Substring(str.IndexOf('@') + 1);
      if (hostFromUrl.IndexOf('/') != -1)
        hostFromUrl = hostFromUrl.Substring(0, hostFromUrl.IndexOf('/'));
      return hostFromUrl;
    }

    public void CheckPermitted(GeneralName name)
    {
      switch (name.TagNo)
      {
        case 1:
          this.CheckPermittedEmail(this.permittedSubtreesEmail, this.ExtractNameAsString(name));
          break;
        case 2:
          this.CheckPermittedDNS(this.permittedSubtreesDNS, DerIA5String.GetInstance((object) name.Name).GetString());
          break;
        case 4:
          this.CheckPermittedDN(Asn1Sequence.GetInstance((object) name.Name.ToAsn1Object()));
          break;
        case 6:
          this.CheckPermittedURI(this.permittedSubtreesURI, DerIA5String.GetInstance((object) name.Name).GetString());
          break;
        case 7:
          this.CheckPermittedIP(this.permittedSubtreesIP, Asn1OctetString.GetInstance((object) name.Name).GetOctets());
          break;
      }
    }

    public void CheckExcluded(GeneralName name)
    {
      switch (name.TagNo)
      {
        case 1:
          this.CheckExcludedEmail(this.excludedSubtreesEmail, this.ExtractNameAsString(name));
          break;
        case 2:
          this.checkExcludedDNS(this.excludedSubtreesDNS, DerIA5String.GetInstance((object) name.Name).GetString());
          break;
        case 4:
          this.CheckExcludedDN(Asn1Sequence.GetInstance((object) name.Name.ToAsn1Object()));
          break;
        case 6:
          this.checkExcludedURI(this.excludedSubtreesURI, DerIA5String.GetInstance((object) name.Name).GetString());
          break;
        case 7:
          this.checkExcludedIP(this.excludedSubtreesIP, Asn1OctetString.GetInstance((object) name.Name).GetOctets());
          break;
      }
    }

    public void IntersectPermittedSubtree(Asn1Sequence permitted)
    {
      // Original code which always throws an exception.
      
      // IDictionary<int, ISet<GeneralSubtree>> dictionary = (IDictionary<int, ISet<GeneralSubtree>>) new Dictionary<int, ISet<GeneralSubtree>>();
      // foreach (object obj in permitted)
      // {
      //   GeneralSubtree instance = GeneralSubtree.GetInstance(obj);
      //   int tagNo = instance.Base.TagNo;
      //   if (dictionary[tagNo] == null)
      //     dictionary[tagNo] = (ISet<GeneralSubtree>) new HashSet<GeneralSubtree>();
      //   dictionary[tagNo].Add(instance);
      // }
      // IEnumerator<KeyValuePair<int, ISet<GeneralSubtree>>> enumerator = dictionary.GetEnumerator();
      
      Dictionary<int, HashSet<GeneralSubtree>> subtreesMap = new Dictionary<int, HashSet<GeneralSubtree>>();
      foreach (object obj in permitted)
      {
        GeneralSubtree instance = GeneralSubtree.GetInstance(obj);
        int tagNo = instance.Base.TagNo;
        HashSet<GeneralSubtree> subtrees;
        if (!subtreesMap.TryGetValue(tagNo, out subtrees))
        {
          subtrees = new HashSet<GeneralSubtree>();
          subtreesMap[tagNo] = subtrees;
        }

        subtrees.Add(instance);
      }

      IEnumerator<KeyValuePair<int, HashSet<GeneralSubtree>>> enumerator = subtreesMap.GetEnumerator();
      while (enumerator.MoveNext())
      {
        KeyValuePair<int, HashSet<GeneralSubtree>> current = enumerator.Current;
        switch (current.Key)
        {
          case 1:
            this.permittedSubtreesEmail = this.IntersectEmail(this.permittedSubtreesEmail, current.Value);
            continue;
          case 2:
            this.permittedSubtreesDNS = this.intersectDNS(this.permittedSubtreesDNS, current.Value);
            continue;
          case 4:
            this.permittedSubtreesDN = this.IntersectDN(this.permittedSubtreesDN, current.Value);
            continue;
          case 6:
            this.permittedSubtreesURI = this.intersectURI(this.permittedSubtreesURI, current.Value);
            continue;
          case 7:
            this.permittedSubtreesIP = this.IntersectIP(this.permittedSubtreesIP, (ISet) current.Value);
            continue;
          default:
            continue;
        }
      }
    }

    private string ExtractNameAsString(GeneralName name) => DerIA5String.GetInstance((object) name.Name).GetString();

    public void IntersectEmptyPermittedSubtree(int nameType)
    {
      switch (nameType)
      {
        case 1:
          this.permittedSubtreesEmail = (ISet<string>) new HashSet<string>();
          break;
        case 2:
          this.permittedSubtreesDNS = (ISet<string>) new HashSet<string>();
          break;
        case 4:
          this.permittedSubtreesDN = (ISet<Asn1Sequence>) new HashSet<Asn1Sequence>();
          break;
        case 6:
          this.permittedSubtreesURI = (ISet<string>) new HashSet<string>();
          break;
        case 7:
          this.permittedSubtreesIP = (ISet<byte[]>) new HashSet<byte[]>();
          break;
      }
    }

    public void AddExcludedSubtree(GeneralSubtree subtree)
    {
      GeneralName name = subtree.Base;
      switch (name.TagNo)
      {
        case 1:
          this.excludedSubtreesEmail = this.UnionEmail(this.excludedSubtreesEmail, this.ExtractNameAsString(name));
          break;
        case 2:
          this.excludedSubtreesDNS = this.unionDNS(this.excludedSubtreesDNS, this.ExtractNameAsString(name));
          break;
        case 4:
          this.excludedSubtreesDN = this.UnionDN(this.excludedSubtreesDN, (Asn1Sequence) name.Name.ToAsn1Object());
          break;
        case 6:
          this.excludedSubtreesURI = this.unionURI(this.excludedSubtreesURI, this.ExtractNameAsString(name));
          break;
        case 7:
          this.excludedSubtreesIP = this.UnionIP(this.excludedSubtreesIP,
            Asn1OctetString.GetInstance((object) name.Name).GetOctets());
          break;
      }
    }

    private static byte[] Max(byte[] ip1, byte[] ip2)
    {
      for (int index = 0; index < ip1.Length; ++index)
      {
        if (((int) ip1[index] & (int) ushort.MaxValue) > ((int) ip2[index] & (int) ushort.MaxValue))
          return ip1;
      }

      return ip2;
    }

    private static byte[] Min(byte[] ip1, byte[] ip2)
    {
      for (int index = 0; index < ip1.Length; ++index)
      {
        if (((int) ip1[index] & (int) ushort.MaxValue) < ((int) ip2[index] & (int) ushort.MaxValue))
          return ip1;
      }

      return ip2;
    }

    private static int CompareTo(byte[] ip1, byte[] ip2)
    {
      if (Arrays.AreEqual(ip1, ip2))
        return 0;
      return Arrays.AreEqual(PkixNameConstraintValidatorFixed.Max(ip1, ip2), ip1) ? 1 : -1;
    }

    private static byte[] Or(byte[] ip1, byte[] ip2)
    {
      byte[] numArray = new byte[ip1.Length];
      for (int index = 0; index < ip1.Length; ++index)
        numArray[index] = (byte) ((uint) ip1[index] | (uint) ip2[index]);
      return numArray;
    }

    [Obsolete("Use GetHashCode instead")]
    public int HashCode() => this.GetHashCode();

    public override int GetHashCode() =>
      this.HashCollection<Asn1Sequence>((ICollection<Asn1Sequence>) this.excludedSubtreesDN) +
      this.HashCollection<string>((ICollection<string>) this.excludedSubtreesDNS) +
      this.HashCollection<string>((ICollection<string>) this.excludedSubtreesEmail) +
      this.HashCollection<byte[]>((ICollection<byte[]>) this.excludedSubtreesIP) +
      this.HashCollection<string>((ICollection<string>) this.excludedSubtreesURI) +
      this.HashCollection<Asn1Sequence>((ICollection<Asn1Sequence>) this.permittedSubtreesDN) +
      this.HashCollection<string>((ICollection<string>) this.permittedSubtreesDNS) +
      this.HashCollection<string>((ICollection<string>) this.permittedSubtreesEmail) +
      this.HashCollection<byte[]>((ICollection<byte[]>) this.permittedSubtreesIP) +
      this.HashCollection<string>((ICollection<string>) this.permittedSubtreesURI);

    private int HashCollection<T>(ICollection<T> coll)
    {
      if (coll == null)
        return 0;
      int num = 0;
      IEnumerator enumerator = (IEnumerator) coll.GetEnumerator();
      while (enumerator.MoveNext())
      {
        object current = enumerator.Current;
        if (current is byte[])
          num += Arrays.GetHashCode((byte[]) current);
        else
          num += current.GetHashCode();
      }

      return num;
    }

    public override bool Equals(object o)
    {
      if (!(o is PkixNameConstraintValidatorFixed))
        return false;
      PkixNameConstraintValidatorFixed constraintValidator = (PkixNameConstraintValidatorFixed) o;
      return
        this.CollectionsAreEqual<Asn1Sequence>((ICollection<Asn1Sequence>) constraintValidator.excludedSubtreesDN,
          (ICollection<Asn1Sequence>) this.excludedSubtreesDN) &&
        this.CollectionsAreEqual<string>((ICollection<string>) constraintValidator.excludedSubtreesDNS,
          (ICollection<string>) this.excludedSubtreesDNS) &&
        this.CollectionsAreEqual<string>((ICollection<string>) constraintValidator.excludedSubtreesEmail,
          (ICollection<string>) this.excludedSubtreesEmail) &&
        this.CollectionsAreEqual<byte[]>((ICollection<byte[]>) constraintValidator.excludedSubtreesIP,
          (ICollection<byte[]>) this.excludedSubtreesIP) &&
        this.CollectionsAreEqual<string>((ICollection<string>) constraintValidator.excludedSubtreesURI,
          (ICollection<string>) this.excludedSubtreesURI) &&
        this.CollectionsAreEqual<Asn1Sequence>((ICollection<Asn1Sequence>) constraintValidator.permittedSubtreesDN,
          (ICollection<Asn1Sequence>) this.permittedSubtreesDN) &&
        this.CollectionsAreEqual<string>((ICollection<string>) constraintValidator.permittedSubtreesDNS,
          (ICollection<string>) this.permittedSubtreesDNS) &&
        this.CollectionsAreEqual<string>((ICollection<string>) constraintValidator.permittedSubtreesEmail,
          (ICollection<string>) this.permittedSubtreesEmail) &&
        this.CollectionsAreEqual<byte[]>((ICollection<byte[]>) constraintValidator.permittedSubtreesIP,
          (ICollection<byte[]>) this.permittedSubtreesIP) && this.CollectionsAreEqual<string>(
          (ICollection<string>) constraintValidator.permittedSubtreesURI,
          (ICollection<string>) this.permittedSubtreesURI);
    }

    private bool CollectionsAreEqual<T>(ICollection<T> coll1, ICollection<T> coll2)
    {
      if (coll1 == coll2)
        return true;
      if (coll1 == null || coll2 == null || coll1.Count != coll2.Count)
        return false;
      IEnumerator enumerator1 = (IEnumerator) coll1.GetEnumerator();
      while (enumerator1.MoveNext())
      {
        object current1 = enumerator1.Current;
        IEnumerator enumerator2 = (IEnumerator) coll2.GetEnumerator();
        bool flag = false;
        while (enumerator2.MoveNext())
        {
          object current2 = enumerator2.Current;
          if (this.SpecialEquals(current1, current2))
          {
            flag = true;
            break;
          }
        }

        if (!flag)
          return false;
      }

      return true;
    }

    private bool SpecialEquals(object o1, object o2)
    {
      if (o1 == o2)
        return true;
      if (o1 == null || o2 == null)
        return false;
      return o1 is byte[] && o2 is byte[] ? Arrays.AreEqual((byte[]) o1, (byte[]) o2) : o1.Equals(o2);
    }

    private string StringifyIP(byte[] ip)
    {
      string str1 = "";
      for (int index = 0; index < ip.Length / 2; ++index)
        str1 = str1 + ((int) ip[index] & (int) byte.MaxValue).ToString() + ".";
      string str2 = str1.Substring(0, str1.Length - 1) + "/";
      for (int index = ip.Length / 2; index < ip.Length; ++index)
        str2 = str2 + ((int) ip[index] & (int) byte.MaxValue).ToString() + ".";
      return str2.Substring(0, str2.Length - 1);
    }

    private string StringifyIPCollection(ISet<byte[]> ips)
    {
      string str = "" + "[";
      IEnumerator<byte[]> enumerator = ips.GetEnumerator();
      while (enumerator.MoveNext())
        str = str + this.StringifyIP(enumerator.Current) + ",";
      if (str.Length > 1)
        str = str.Substring(0, str.Length - 1);
      return str + "]";
    }

    public override string ToString()
    {
      string str1 = "" + "permitted:\n";
      if (this.permittedSubtreesDN != null)
        str1 = str1 + "DN:\n" + this.permittedSubtreesDN.ToString() + "\n";
      if (this.permittedSubtreesDNS != null)
        str1 = str1 + "DNS:\n" + this.permittedSubtreesDNS.ToString() + "\n";
      if (this.permittedSubtreesEmail != null)
        str1 = str1 + "Email:\n" + this.permittedSubtreesEmail.ToString() + "\n";
      if (this.permittedSubtreesURI != null)
        str1 = str1 + "URI:\n" + this.permittedSubtreesURI.ToString() + "\n";
      if (this.permittedSubtreesIP != null)
        str1 = str1 + "IP:\n" + this.StringifyIPCollection(this.permittedSubtreesIP) + "\n";
      string str2 = str1 + "excluded:\n";
      if (this.excludedSubtreesDN.Count > 0)
        str2 = str2 + "DN:\n" + this.excludedSubtreesDN.ToString() + "\n";
      if (this.excludedSubtreesDNS.Count > 0)
        str2 = str2 + "DNS:\n" + this.excludedSubtreesDNS.ToString() + "\n";
      if (this.excludedSubtreesEmail.Count > 0)
        str2 = str2 + "Email:\n" + this.excludedSubtreesEmail.ToString() + "\n";
      if (this.excludedSubtreesURI.Count > 0)
        str2 = str2 + "URI:\n" + this.excludedSubtreesURI.ToString() + "\n";
      if (this.excludedSubtreesIP.Count > 0)
        str2 = str2 + "IP:\n" + this.StringifyIPCollection(this.excludedSubtreesIP) + "\n";
      return str2;
    }
  }
}