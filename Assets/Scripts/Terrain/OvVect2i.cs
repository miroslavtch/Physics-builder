using System;

public class Vect2i
{
	private const int DEF_X = 0;
	private const int DEF_Z = 0;

	public int X {
		get;
		set;
	}

	public int Z {
		get;
		set;
	}

	public Vect2i () : this(DEF_X, DEF_Z)
	{
		//no code
	}
	public Vect2i (int pX, int pZ)
	{
		this.X = pX;
		this.Z = pZ;
	}

}

