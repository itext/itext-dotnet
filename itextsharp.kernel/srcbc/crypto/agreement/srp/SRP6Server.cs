using System;

using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Org.BouncyCastle.Crypto.Agreement.Srp
{
	/**
	 * Implements the server side SRP-6a protocol. Note that this class is stateful, and therefore NOT threadsafe.
	 * This implementation of SRP is based on the optimized message sequence put forth by Thomas Wu in the paper
	 * "SRP-6: Improvements and Refinements to the Secure Remote Password Protocol, 2002"
	 */
	public class Srp6Server
	{
	    protected BigInteger N;
	    protected BigInteger g;
	    protected BigInteger v;

	    protected SecureRandom random;
	    protected IDigest digest;

	    protected BigInteger A;

	    protected BigInteger privB;
	    protected BigInteger pubB;

	    protected BigInteger u;
	    protected BigInteger S;

	    public Srp6Server()
	    {
	    }

	    /**
	     * Initialises the server to accept a new client authentication attempt
	     * @param N The safe prime associated with the client's verifier
	     * @param g The group parameter associated with the client's verifier
	     * @param v The client's verifier
	     * @param digest The digest algorithm associated with the client's verifier
	     * @param random For key generation
	     */
	    public virtual void Init(BigInteger N, BigInteger g, BigInteger v, IDigest digest, SecureRandom random)
	    {
	        this.N = N;
	        this.g = g;
	        this.v = v;

	        this.random = random;
	        this.digest = digest;
	    }

	    /**
	     * Generates the server's credentials that are to be sent to the client.
	     * @return The server's public value to the client
	     */
	    public virtual BigInteger GenerateServerCredentials()
	    {
	        BigInteger k = Srp6Utilities.CalculateK(digest, N, g);
	        this.privB = SelectPrivateValue();
	    	this.pubB = k.Multiply(v).Mod(N).Add(g.ModPow(privB, N)).Mod(N);

	        return pubB;
	    }

	    /**
	     * Processes the client's credentials. If valid the shared secret is generated and returned.
	     * @param clientA The client's credentials
	     * @return A shared secret BigInteger
	     * @throws CryptoException If client's credentials are invalid
	     */
	    public virtual BigInteger CalculateSecret(BigInteger clientA)
	    {
	        this.A = Srp6Utilities.ValidatePublicValue(N, clientA);
	        this.u = Srp6Utilities.CalculateU(digest, N, A, pubB);
	        this.S = CalculateS();

	        return S;
	    }

	    protected virtual BigInteger SelectPrivateValue()
	    {
	    	return Srp6Utilities.GeneratePrivateValue(digest, N, g, random);    	
	    }

		private BigInteger CalculateS()
	    {
			return v.ModPow(u, N).Multiply(A).Mod(N).ModPow(privB, N);
	    }
	}
}
